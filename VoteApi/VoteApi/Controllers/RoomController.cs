using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoteApi.Data;
using VoteApi.Infrastructure;
using VoteApi.Models;

namespace VoteApi.Controllers
{
	[Route(_ClassRoute)]
	[ApiController]
	public class RoomController : ControllerBase
	{
		private const string _ClassRoute = "api/" + Constants.RoomsRoute;
		private const string _ParticipantsByRoomRouteParameter = "{roomId}/" + Constants.ParticipantsRoute;

		private readonly ApiContext _context;

		public RoomController(ApiContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<List<Room>>> GetAsync(CancellationToken cancellationToken)
		{
			return Ok(await _context.Rooms.AsNoTracking().ToListAsync(cancellationToken));
		}

		[HttpPost]
		public async Task<ActionResult<Room>> PostAsync([FromBody] Room room, CancellationToken cancellationToken)
		{
			var result = _context.Rooms.Add(room);
			await _context.SaveChangesAsync(cancellationToken);
			return result.Entity;
		}

		[HttpGet(_ParticipantsByRoomRouteParameter)]
		public async Task<ActionResult<List<Participant>>> GetAllAsync([FromRoute]int roomId, CancellationToken cancellationToken)
		{
			var participants = await _context.Participants.AsNoTracking().Where(Participant.GetByRoomId(roomId)).ToListAsync(cancellationToken);
			return participants;
		}
	}
}
