using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace VoteApi.Test
{
	[Trait("TestCategory", "Controller - Participants")]
	[Trait("Category", "Controller - Participants")]
	public class ParticipantControllerTests : IClassFixture<InMemoryApplicationFactory<Startup>>
	{
		private readonly HttpClient _client;
		private readonly InMemoryApplicationFactory<Startup> _factory;

		public ParticipantControllerTests(InMemoryApplicationFactory<Startup> factory)
		{
			_client = factory.CreateClient();
			_factory = factory;
		}

		[Fact]
		public async Task CanPostParticipant_ToExistingRoom()
		{
			var room = await _client.CreateRoomAsync("Room Name");
			var savedParticipant = await _client.JoinParticipantToRoomAsync(room.Id, "bob");

			savedParticipant.RoomId.Should().Be(room.Id);
			savedParticipant.Name.Should().Be("bob");
			savedParticipant.IsRaised.Should().Be(false);
		}

		[Fact]
		public void CanNotPostParticipant_ToNonExistantRoom()
		{
			new Func<Task>(() => _client.JoinParticipantToRoomAsync(int.MaxValue, "bob"))
				.Should()
				.Throw<HttpRequestException>().WithMessage("*400*");
		}

		[Fact]
		public async Task CanNotUpdateParticipantRoomOrId_OnceAddedToARoom()
		{
			var room = await _client.CreateRoomAsync("Room Name");
			var participant = await _client.JoinParticipantToRoomAsync(room.Id, "Bob");
			var originalId = participant.Id;

			participant.RoomId++;
			participant.Id++;
			var updatedParticipant = await _client.UpdateParticipantAsync(originalId, participant);
			var fetchedParticipant = await _client.GetParticipantAsync(originalId);

			updatedParticipant.RoomId.Should().NotBe(participant.RoomId);
			updatedParticipant.Id.Should().NotBe(participant.Id);
			fetchedParticipant.RoomId.Should().NotBe(participant.RoomId);
			fetchedParticipant.Id.Should().NotBe(participant.Id);
		}

		[Fact]
		public async Task CanUpdateParticipantNameAndIsRaised()
		{
			var room = await _client.CreateRoomAsync("Room Name");
			var participant = await _client.JoinParticipantToRoomAsync(room.Id, "Bob");

			participant.IsRaised = !participant.IsRaised;
			participant.Name = "Biff";
			var updatedParticipant = await _client.UpdateParticipantAsync(participant.Id, participant);
			var fetchedParticipant = await _client.GetParticipantAsync(participant.Id);

			updatedParticipant.IsRaised.Should().Be(participant.IsRaised);
			updatedParticipant.Name.Should().Be(participant.Name);
			fetchedParticipant.IsRaised.Should().Be(participant.IsRaised);
			fetchedParticipant.Name.Should().Be(participant.Name);
		}


		[Fact]
		public async Task ParticipantCanLeaveRoom()
		{
			var room = await _client.CreateRoomAsync("Room Name");
			var participant = await _client.JoinParticipantToRoomAsync(room.Id, "Bob");
			var removedParticipant = await _client.DeleteParticipantAsync(participant.Id);

			new Func<Task>(() => _client.GetParticipantAsync(participant.Id))
				.Should()
				.Throw<HttpRequestException>().WithMessage("*404*");
			removedParticipant.Id.Should().Be(participant.Id);
		}

		[Fact]
		public async Task ParticipantCanLeaveRoom_MultipleTimesSafely()
		{
			var room = await _client.CreateRoomAsync("Room Name");
			var participant = await _client.JoinParticipantToRoomAsync(room.Id, "Bob");


			var removedParticipant = await _client.DeleteParticipantAsync(participant.Id);
			removedParticipant.Id.Should().Be(participant.Id);
			removedParticipant = await _client.DeleteParticipantAsync(participant.Id);
			removedParticipant.Should().BeNull();
		}
	}
}
