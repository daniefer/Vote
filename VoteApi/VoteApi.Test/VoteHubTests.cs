using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using VoteApi.Hubs;
using Xunit;

namespace VoteApi.Test
{
	[Trait("TestCategory", "Controller - Rooms")]
	[Trait("Category", "Controller - Rooms")]
	public class VoteHubTests : IClassFixture<InMemoryApplicationFactory<Startup>>
	{
		private readonly HttpClient _client;
		private readonly InMemoryApplicationFactory<Startup> _factory;

		public VoteHubTests(InMemoryApplicationFactory<Startup> factory)
		{
			_client = factory.CreateClient();
			_factory = factory;
		}

		private async Task<HubConnection> ConnectToHubAsync(Action<ParticipantEventArgs> onParticipantChanged, Action<RoomEventArgs> onRoomChanged)
		{
			var connection = new HubConnectionBuilder()
				.WithUrl(new Uri(_factory.Server.BaseAddress, "voteHub"), opts => opts.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler())
				.Build();
			if (onParticipantChanged != null)
				connection.On(nameof(IClient.ParticipantChanged), onParticipantChanged);
			if (onRoomChanged != null)
				connection.On(nameof(IClient.RoomChanged), onRoomChanged);
			await connection.StartAsync();
			return connection;
		}

		private Task JoinRoomAsync(HubConnection connection, int roomId, int participantId)
		{
			return connection.InvokeAsync(nameof(VoteHub.JoinRoomAsync), roomId, participantId);
		}

		[Fact]
		public async Task HubShouldFireParticipantEvent_WhenParticipantJoinsRoom()
		{
			var room = await _client.CreateRoomAsync("Hub Test Room");
			var participantReceivedHandler = new AutoResetEvent(false);
			var participantChanges = new List<ParticipantEventArgs>();
			var connection = await ConnectToHubAsync(p =>
			{
				participantChanges.Add(p);
				participantReceivedHandler.Set();
			}
			, null);

			var participant1 = await _client.JoinParticipantToRoomAsync(room.Id, "David");
			await JoinRoomAsync(connection, room.Id, participant1.Id);
			var participant2 = await _client.JoinParticipantToRoomAsync(room.Id, "Josh");

			participantReceivedHandler.WaitOne(TimeSpan.FromMilliseconds(500));
			participantChanges.Should().HaveCount(1);
			participantChanges[0].Event.Should().Be(ParticipantEvent.Joined);
			participantChanges[0].Participant.Should().NotBeNull();
			participantChanges[0].Participant.Id.Should().Be(participant2.Id);
		}

		[Fact]
		public async Task HubShouldFireParticipantEvent_WhenParticipantLeaveRoom()
		{
			var room = await _client.CreateRoomAsync("Hub Test Room");
			var participantReceivedHandler = new AutoResetEvent(false);
			var participantChanges = new List<ParticipantEventArgs>();
			var connection = await ConnectToHubAsync(p =>
			{
				participantChanges.Add(p);
				participantReceivedHandler.Set();
			}
			, null);

			var participant1 = await _client.JoinParticipantToRoomAsync(room.Id, "David");
			await JoinRoomAsync(connection, room.Id, participant1.Id);
			var participant2 = await _client.JoinParticipantToRoomAsync(room.Id, "Josh");

			participantReceivedHandler.WaitOne(TimeSpan.FromMilliseconds(500));
			await _client.DeleteParticipantAsync(participant2.Id);
			participantReceivedHandler.WaitOne(TimeSpan.FromMilliseconds(500));

			participantChanges.Should().HaveCount(2);
			participantChanges[1].Event.Should().Be(ParticipantEvent.Left);
			participantChanges[1].Participant.Should().NotBeNull();
			participantChanges[1].Participant.Id.Should().Be(participant2.Id);
		}

		[Fact]
		public async Task HubShouldFireRoomEvent_WhenAllParticipantsLeave()
		{
			var room = await _client.CreateRoomAsync("Hub Test Room");
			var roomReceivedHandler = new AutoResetEvent(false);
			var roomChanges = new List<RoomEventArgs>();
			var connection = await ConnectToHubAsync(null, r =>
			{
				roomChanges.Add(r);
				roomReceivedHandler.Set();
			});

			var participant1 = await _client.JoinParticipantToRoomAsync(room.Id, "David");
			await JoinRoomAsync(connection, room.Id, participant1.Id);
			var participant2 = await _client.JoinParticipantToRoomAsync(room.Id, "Josh");

			await _client.DeleteParticipantAsync(participant1.Id);
			await _client.DeleteParticipantAsync(participant2.Id);
			roomReceivedHandler.WaitOne(TimeSpan.FromMilliseconds(500));

			roomChanges.Should().HaveCount(1);
			roomChanges[0].Room.Should().NotBeNull();
			roomChanges[0].Room.Id.Should().Be(room.Id);
		}
	}
}
