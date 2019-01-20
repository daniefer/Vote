using VoteApi.Models;

namespace VoteApi.Hubs
{
	public interface IClient
	{
		void ParticipantChanged(ParticipantEventArgs eventArgs);

		void RoomChanged(RoomEventArgs roomEventArgs);
	}
}
