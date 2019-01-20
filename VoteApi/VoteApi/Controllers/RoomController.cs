using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
	public class RoomController : ControllerBase
	{
		private const string _ClassRoute = "api/" + Constants.RoomsRoute;
		private const string _RoomRouteParameter = "{roomId}";
		private const string _ParticipantsByRoomRouteParameter = _RoomRouteParameter + "/" + Constants.ParticipantsRoute;
		private const string _ResetParticipantsByRoomRouteParameter = _ParticipantsByRoomRouteParameter + "/_reset";

		private readonly ApiContext _context;
		private readonly IHubContext<VoteHub> _voteHub;
		private readonly ILogger<RoomController> _logger;

		public RoomController(ApiContext context, IHubContext<VoteHub> voteHub, ILogger<RoomController> logger)
		{
			_context = context;
			_voteHub = voteHub;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<List<Room>>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.Rooms.Include(r => r.Participants).AsNoTracking().ToListAsync(cancellationToken);
		}

		[HttpGet(_RoomRouteParameter)]
		public async Task<ActionResult<Room>> GetAsync([FromRoute]int roomId, CancellationToken cancellationToken)
		{
			return await _context.Rooms.Where(x => x.Id == roomId).Include(r => r.Participants).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
		}

		[HttpPost(_RoomRouteParameter)]
		public async Task<ActionResult<Room>> PostAsync([FromRoute]int roomId, [Bind(nameof(Room.Name))] Room room, CancellationToken cancellationToken)
		{
			var result = await _context.Rooms.FirstOrDefaultAsync(Room.GetById(roomId), cancellationToken);
			if (result.Name != room.Name)
			{
				result.Name = room.Name;
				await _context.SaveChangesAsync(cancellationToken);
				await _voteHub.RoomChanged(result, cancellationToken);
			}
			return result;
		}

		[HttpPost]
		public async Task<ActionResult<Room>> PostAsync([Bind(nameof(Room.Name))] Room room, CancellationToken cancellationToken)
		{
			var result = _context.Rooms.Add(room);
			await _context.SaveChangesAsync(cancellationToken);
			return result.Entity;
		}

		[HttpPost(_ParticipantsByRoomRouteParameter)]
		public async Task<ActionResult<Participant>> PostJoinParticipantToRoomAsync([FromRoute]int roomId, [Bind(ParticipantController.ParticipantBindingFilter)] Participant participant, CancellationToken cancellationToken)
		{
			participant.RoomId = roomId;
			if (!await _context.Rooms.AnyAsync(x => x.Id == roomId))
			{
				return BadRequest();
			}
			_context.Participants.Add(participant);
			await _context.SaveChangesAsync(cancellationToken);
			await _voteHub.ParticipantJoined(participant, cancellationToken);
			return participant;
		}

		[HttpPost(_ResetParticipantsByRoomRouteParameter)]
		public async Task<ActionResult<Room>> PostResetParticipantsHandsAsync([FromRoute]int roomId, CancellationToken cancellationToken)
		{
			var room = await _context.Rooms.Include(x => x.Participants).FirstOrDefaultAsync(Room.GetById(roomId));

			foreach (var participant in room.Participants)
			{
				participant.IsRaised = false;
			}
			await _context.SaveChangesAsync(cancellationToken);
			await _voteHub.RoomChanged(room, cancellationToken);
			return room;
		}
	}
}
