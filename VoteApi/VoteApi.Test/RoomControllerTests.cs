using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VoteApi.Models;
using Xunit;

namespace VoteApi.Test
{
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
            var room = new Room { Name = "Test Room" };
            var json = JsonConvert.SerializeObject(room);
            var response = await _client.PostAsync("api/Room", new StringContent(json, Encoding.Default, "application/json"));
            var body = await response.Content.ReadAsStringAsync();
            var newRoom = JsonConvert.DeserializeObject<Room>(body);

            response.EnsureSuccessStatusCode();
            Assert.NotNull(newRoom);
            Assert.Equal(1, newRoom.Id);
            Assert.Equal(room.Name, newRoom.Name);
        }
    }
}
