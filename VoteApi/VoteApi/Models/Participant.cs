using System;
using System.Linq.Expressions;

namespace VoteApi.Models
{
    public class Participant
    {
        public Participant(int roomId, string name, bool isRaised)
        {
            RoomId = roomId;
            Name = name;
            IsRaised = isRaised;
        }

        public int Id { get; set; }

        public int RoomId { get; set; }

        public string Name { get; set; }

        public bool IsRaised { get; set; }

        public static Expression<Func<Participant, bool>> GetByRoomId(int roomId) => x => x.RoomId == roomId;
        public static Expression<Func<Participant, bool>> GetById(int participantId) => x => x.Id.Equals(participantId);
    }
}
