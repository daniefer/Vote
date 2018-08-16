using System;

namespace VoteApi.Infrastructure
{
    public class RoomContainsMembersException : Exception
    {
        public RoomContainsMembersException(string roomId) : base(roomId)
        {

        }
    }
}
