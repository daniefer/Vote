using VoteApi.Models;

namespace VoteApi.Hubs
{
	public class ParticipantEventArgs
	{
		public ParticipantEvent Event { get; set; }

		public Participant Participant { get; set; }
	}
}
