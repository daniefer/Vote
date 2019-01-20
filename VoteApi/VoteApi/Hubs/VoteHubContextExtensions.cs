using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VoteApi.Models;

namespace VoteApi.Hubs
{
	public static class VoteHubContextExtensions
	{
		public static Task RoomChanged(this IHubContext<VoteHub> context, Room room, CancellationToken cancellationToken)
		{
			var eventArgs = new RoomEventArgs { Event = RoomEvent.Changed, Room = room };
			return context.Clients.Group(room.Id.ToString()).SendAsync(nameof(IClient.RoomChanged), eventArgs, cancellationToken);
		}

		public static Task RoomDeleted(this IHubContext<VoteHub> context, Room room, CancellationToken cancellationToken)
		{
			var eventArgs = new RoomEventArgs { Event = RoomEvent.Deleted, Room = room };
			return context.Clients.Group(room.Id.ToString()).SendAsync(nameof(IClient.RoomChanged), eventArgs, cancellationToken);
		}

		public static Task ParticipantLeft(this IHubContext<VoteHub> context, Participant participant, CancellationToken cancellationToken)
		{
			var eventArgs = new ParticipantEventArgs { Event = ParticipantEvent.Left, Participant = participant };
			return context.Clients.Group(participant.RoomId.ToString()).SendAsync(nameof(IClient.ParticipantChanged), eventArgs, cancellationToken);
		}

		public static Task ParticipantJoined(this IHubContext<VoteHub> context, Participant participant, CancellationToken cancellationToken)
		{
			var eventArgs = new ParticipantEventArgs { Event = ParticipantEvent.Joined, Participant = participant };
			return context.Clients.Group(participant.RoomId.ToString()).SendAsync(nameof(IClient.ParticipantChanged), eventArgs, cancellationToken);
		}

		public static Task ParticipantChanged(this IHubContext<VoteHub> context, Participant participant, CancellationToken cancellationToken)
		{
			var eventArgs = new ParticipantEventArgs { Event = ParticipantEvent.Changed, ParticipantChanged = participant };
			return context.Clients.Group(participant.RoomId.ToString()).SendAsync(nameof(IClient.RoomChanged), eventArgs, cancellationToken);
		}
	}
}
