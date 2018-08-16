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
    public class ParticipantController : ControllerBase
    {
        private const string _ClassRoute = "api/" + Constants.ParticipantsRoute;
        private const string _ParticipantIdRouteParameter = "{participantId}";

        private readonly ApiContext _context;

        public ParticipantController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet(_ParticipantIdRouteParameter)]
        public async Task<ActionResult<Participant>> GetAsync([FromRoute] int participantId, CancellationToken cancellationToken)
        {
            var participant = await _context.Participants.AsNoTracking().FirstOrDefaultAsync(Participant.GetById(participantId), cancellationToken);
            if (participant == null)
                throw new NotFoundException<Participant>();
            return participant;
        }

        [HttpPost]
        public async Task<ActionResult<Participant>> PostAsync([FromBody] Participant participant, CancellationToken cancellationToken)
        {
            var result = _context.Participants.Add(participant);
            await _context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        [HttpDelete(_ParticipantIdRouteParameter)]
        public async Task DeleteAsync(int participantId, CancellationToken cancellationToken)
        {
            var participant = await _context.Participants.FirstOrDefaultAsync(Participant.GetById(participantId), cancellationToken);
            if (participant == null)
                return;
            var room = await _context.Rooms.FirstOrDefaultAsync(Room.GetById(participant.RoomId), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}