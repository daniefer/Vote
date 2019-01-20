using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VoteApi.Infrastructure;
using VoteApi.Models;

namespace VoteApi.Test
{
	public static class RoomHttpClientExtensions
	{
		public static async Task<List<Room>> GetRoomsAsync(this HttpClient client)
		{
			var response = await client.GetAsync($"api/{Constants.RoomsRoute}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<List<Room>>();
		}

		public static async Task<Room> GetRoomAsync(this HttpClient client, int roomId)
		{
			var response = await client.GetAsync($"api/{Constants.RoomsRoute}/{roomId}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Room>();
		}

		public static async Task<Room> CreateRoomAsync(this HttpClient client, string name)
		{
			var room = new Room { Name = name };
			var response = await client.PostAsJsonAsync($"api/{Constants.RoomsRoute}", room);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Room>();
		}

		public static async Task<Room> ResetRoomAsync(this HttpClient client, int roomId)
		{
			var response = await client.PostAsync($"api/{Constants.RoomsRoute}/{roomId}/{Constants.ParticipantsRoute}/_reset", null);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Room>();
		}


		public static async Task<Room> UpdateRoomAsync(this HttpClient client, int roomId, Room room)
		{
			var response = await client.PostAsJsonAsync($"api/{Constants.RoomsRoute}/{roomId}", room);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Room>();
		}
	}
}
