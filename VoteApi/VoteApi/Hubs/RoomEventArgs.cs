using VoteApi.Models;

namespace VoteApi.Hubs
{
	public class RoomEventArgs
	{
		public RoomEvent Event { get; set; }

		public Room Room { get; set; }
	}
}
