namespace VoteApi.Hubs
{
    public class RoomEvent
    {
        public RoomEvent(int roomId, ConnectionEvent @event, int participantId, bool isRaised)
        {
            RoomId = roomId;
            Event = @event;
            ParticipantId = participantId;
            IsRaised = isRaised;
        }

        public int RoomId { get; private set; }

        public ConnectionEvent Event { get; private set; }

        public int ParticipantId { get; private set; }

        public bool IsRaised { get; set; }
    }
}
