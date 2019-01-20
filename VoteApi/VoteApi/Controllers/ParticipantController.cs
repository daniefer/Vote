using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteApi.Data;
using VoteApi.Hubs;
using VoteApi.Infrastructure;
using VoteApi.Models;

namespace VoteApi.Controllers
{
	[Route(_ClassRoute)]
	[ApiController]
	public class ParticipantController : ControllerBase
	{
		public const string ParticipantBindingFilter = nameof(Participant.IsRaised) + "," + nameof(Participant.Name);
		private const string _ClassRoute = "api/" + Constants.ParticipantsRoute;
		private const string _ParticipantIdRouteParameter = "{participantId}";

		private readonly ApiContext _context;
		private readonly IHubContext<VoteHub> _voteHub;
		private readonly ILogger<ParticipantController> _logger;

		public ParticipantController(ApiContext context, IHubContext<VoteHub> voteHub, ILogger<ParticipantController> logger)
		{
			_context = context;
			_voteHub = voteHub;
			_logger = logger;
		}

		[HttpGet(_ParticipantIdRouteParameter)]
		public async Task<ActionResult<Participant>> GetAsync([FromRoute] int participantId, CancellationToken cancellationToken)
		{
			var participant = await _context.Participants.AsNoTracking().FirstOrDefaultAsync(Participant.GetById(participantId), cancellationToken);
			if (participant == null)
				return NotFound();
			return participant;
		}

		[HttpPost(_ParticipantIdRouteParameter)]
		public async Task<ActionResult<Participant>> PostAsync([FromRoute]int participantId, [Bind(ParticipantBindingFilter)] Participant participant, CancellationToken cancellationToken)
		{
			var result = await _context.Participants.FirstOrDefaultAsync(x => x.Id == participantId);
			if (result.IsRaised != participant.IsRaised || result.Name != participant.Name)
			{
				result.IsRaised = participant.IsRaised;
				result.Name = participant.Name;
				await _context.SaveChangesAsync(cancellationToken);
				await _voteHub.ParticipantChanged(result, cancellationToken);
			}
			return result;
		}

		[HttpDelete(_ParticipantIdRouteParameter)]
		public async Task<ActionResult<Participant>> DeleteAsync(int participantId, CancellationToken cancellationToken)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				var participant = await _context.Participants.FirstOrDefaultAsync(Participant.GetById(participantId), cancellationToken);
				if (participant == null)
				{
					return null;
				}

				Transaction.Current.EnlistVolatile(new TransactionEnlistmentNotification(() => _voteHub.ParticipantLeft(participant, cancellationToken)), EnlistmentOptions.None);
				_context.Participants.Remove(participant);
				await _context.SaveChangesAsync(cancellationToken);


				var room = await _context.Rooms.Include(x => x.Participants).FirstOrDefaultAsync(Room.GetById(participant.RoomId), cancellationToken);
				if (room == null)
				{
					return participant;
				}

				if (!room.Participants.Any())
				{
					Transaction.Current.EnlistVolatile(new TransactionEnlistmentNotification(() => _voteHub.RoomDeleted(room, cancellationToken)), EnlistmentOptions.None);
					_context.Rooms.Remove(room);
					await _context.SaveChangesAsync(cancellationToken);
				}
				scope.Complete();
				return participant;
			}
		}
	}
}