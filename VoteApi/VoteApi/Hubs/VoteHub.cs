using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VoteApi.Data;
using VoteApi.Infrastructure;
using VoteApi.Models;

namespace VoteApi.Hubs
{
    public class VoteHub : Hub
    {
        private const string ParticipantIdContextKey = "participantId";

        private readonly ApiContext _context;

        public VoteHub(ApiContext context)
        {
            _context = context;
        }

        public async Task ToggleHand(CancellationToken cancellationToken)
        {
            var participantId = GetParticipantId();
            var participant = await _context.Participants.FirstOrDefaultAsync(Participant.GetById(participantId), cancellationToken);
            if (participant == null)
                throw new NotFoundException<Participant>();
            participant.IsRaised = !participant.IsRaised;
            await _context.SaveChangesAsync(cancellationToken);
            await NotifyRoom(new RoomEvent(participant.RoomId, ConnectionEvent.HandToggled, participant.Id, participant.IsRaised), cancellationToken);
        }

        public async Task JoinRoomAsync(int roomId, string name, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(Room.GetById(roomId), cancellationToken);
            if (room == null)
                throw new NotFoundException<Room>();
            var result = _context.Participants.Add(new Participant(room.Id, name, false));
            await _context.SaveChangesAsync(cancellationToken);
            SetParticipantId(result.Entity.Id);
            await NotifyRoom(new RoomEvent(room.Id, ConnectionEvent.Joined, result.Entity.Id, result.Entity.IsRaised), cancellationToken);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var participantId = GetParticipantId();
            var participant = await _context.Participants.FirstOrDefaultAsync(Participant.GetById(participantId));
            if (participant != null)
                _context.Participants.Remove(participant);
            await Task.WhenAll(_context.SaveChangesAsync(), base.OnDisconnectedAsync(exception), NotifyRoom(new RoomEvent(participant.RoomId, ConnectionEvent.Left, participantId, false)));
        }

        private Task NotifyRoom(RoomEvent roomEvent, CancellationToken? cancellationToken = null)
        {
            return Clients.Group(roomEvent.RoomId.ToString()).SendAsync(nameof(NotifyRoom), roomEvent, cancellationToken);
        }

        private int GetParticipantId()
        {
            if (!Context.Items.TryGetValue(ParticipantIdContextKey, out var value))
                throw new NotFoundException<Participant>();
            return (int)value;
        }

        private void SetParticipantId(int participantId)
        {
            Context.Items.Add(ParticipantIdContextKey, participantId);
        }
    }
}
