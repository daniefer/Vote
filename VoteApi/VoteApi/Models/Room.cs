using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VoteApi.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Participant> Participants { get; set; }

        public static Expression<Func<Room, bool>> GetById(int roomId) => x => x.Id == roomId;
    }
}
