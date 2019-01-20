using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace VoteApi.Test
{

	[Trait("TestCategory", "Controller - Rooms")]
	[Trait("Category", "Controller - Rooms")]
	public class RoomControllerTests : IClassFixture<InMemoryApplicationFactory<Startup>>
	{
		private readonly HttpClient _client;
		private readonly InMemoryApplicationFactory<Startup> _factory;

		public RoomControllerTests(InMemoryApplicationFactory<Startup> factory)
		{
			_client = factory.CreateClient();
			_factory = factory;
		}

		[Fact]
		public async Task CanCreateRoomAsync()
		{
			var roomName = "test room";
			var savedRoom = await _client.CreateRoomAsync(roomName);

			savedRoom.Should().NotBeNull();
			savedRoom.Id.Should().NotBe(default(int));
			savedRoom.Name.Should().Be(roomName);
		}

		[Fact]
		public async Task CanRequestRoomsAsync()
		{
			await _client.CreateRoomAsync("Room 1");
			await _client.CreateRoomAsync("Room 2");

			var existingRooms = await _client.GetRoomsAsync();

			existingRooms.Should().NotBeNull();
			existingRooms.Count.Should().BeGreaterOrEqualTo(2);
			existingRooms.Count.Should().Be(existingRooms.Select(x => x.Id).Distinct().Count());
		}

		[Fact]
		public async Task CanUpdateRoomNameAsync()
		{
			var room = await _client.CreateRoomAsync("Room 1");
			var roomId = room.Id;
			room.Name = "Room 2";
			room = await _client.UpdateRoomAsync(room.Id, room);
			var fetchedRoom = await _client.GetRoomAsync(room.Id);

			room.Should().NotBeNull();
			room.Id.Should().Be(roomId);
			room.Name.Should().Be("Room 2");
			fetchedRoom.Should().NotBeNull();
			fetchedRoom.Id.Should().Be(roomId);
			fetchedRoom.Name.Should().Be("Room 2");
		}

		[Fact]
		public async Task CanRequestRoomAsync()
		{
			var roomName = "participant test room";
			var savedRoom = await _client.CreateRoomAsync(roomName);

			var room = await _client.GetRoomAsync(savedRoom.Id);

			room.Should().NotBeNull();
			room.Id.Should().Be(savedRoom.Id);
			room.Name.Should().Be(roomName);
		}


		[Fact]
		public async Task CanResetRoomMembersHandsAsync()
		{
			var roomName = "participant test room";
			var savedRoom = await _client.CreateRoomAsync(roomName);
			var participant1 = await _client.JoinParticipantToRoomAsync(savedRoom.Id, "Susan", true);
			var participant2 = await _client.JoinParticipantToRoomAsync(savedRoom.Id, "Becky", true);
			var participant3 = await _client.JoinParticipantToRoomAsync(savedRoom.Id, "Amanda", true);
			var roomBeforeReset = await _client.GetRoomAsync(savedRoom.Id);

			var room = await _client.ResetRoomAsync(savedRoom.Id);

			var roomAfterReset = await _client.GetRoomAsync(savedRoom.Id);

			roomBeforeReset.Participants.Where(x => x.IsRaised).Should().HaveCount(roomBeforeReset.Participants.Count);
			room.Participants.Where(x => x.IsRaised).Should().HaveCount(0);
			roomAfterReset.Participants.Where(x => x.IsRaised).Should().HaveCount(0);
		}
	}
}
