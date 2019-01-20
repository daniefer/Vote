using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VoteApi.Data;
using VoteApi.Models;

namespace VoteApi.Hubs
{
	public class VoteHub : Hub
	{
		private const string ParticipantIdContextKey = "participantId";
		private const string RoomIdContextKey = "roomId";

		private readonly ApiContext _context;

		public VoteHub(ApiContext context)
		{
			_context = context;
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			if (Context.Items.TryGetValue(RoomIdContextKey, out var roomId) && Context.Items.TryGetValue(ParticipantIdContextKey, out var participantId))
			{
				await NotifyRoomParticipantLeft((int)roomId, (int)participantId, CancellationToken.None);
			}
			await base.OnDisconnectedAsync(exception);
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
		}

		public async Task JoinRoomAsync(int roomId, int participantId)
		{
			Context.Items.Add(ParticipantIdContextKey, participantId);
			Context.Items.Add(RoomIdContextKey, roomId);
			await this.Groups.AddToGroupAsync(this.Context.ConnectionId, roomId.ToString());
			await NotifyRoomParticipantJoined(roomId, participantId, CancellationToken.None);
		}

		private async Task NotifyRoomParticipantLeft(int roomId, int participantId, CancellationToken cancellationToken)
		{
			var participant = await _context.Participants.FirstOrDefaultAsync(Participant.GetById(participantId), cancellationToken);

			var eventArgs = new ParticipantEventArgs { Event = ParticipantEvent.Left, Participant = participant };
			await Clients.GroupExcept(roomId.ToString(), new[] { Context.ConnectionId }).SendAsync(nameof(IClient.ParticipantChanged), eventArgs);
		}
		private async Task NotifyRoomParticipantJoined(int roomId, int participantId, CancellationToken cancellationToken)
		{
			var participant = await _context.Participants.FirstOrDefaultAsync(Participant.GetById(participantId), cancellationToken);
			var eventArgs = new ParticipantEventArgs { Event = ParticipantEvent.Joined, Participant = participant };
			await Clients.GroupExcept(roomId.ToString(), new[] { Context.ConnectionId }).SendAsync(nameof(IClient.ParticipantChanged), eventArgs);
		}
	}
}
