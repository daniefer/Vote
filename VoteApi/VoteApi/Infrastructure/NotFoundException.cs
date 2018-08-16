using System;

namespace VoteApi.Infrastructure
{
    public class NotFoundException<T> : Exception
    {
        public NotFoundException() : base(typeof(T).Name)
        {

        }
    }
}
