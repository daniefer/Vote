using System.Net.Http;
using System.Threading.Tasks;
using VoteApi.Infrastructure;
using VoteApi.Models;

namespace VoteApi.Test
{
	public static class ParticipantHttpClientExtensions
	{
		public static async Task<Participant> GetParticipantAsync(this HttpClient client, int participantId)
		{
			var response = await client.GetAsync($"api/{Constants.ParticipantsRoute}/{participantId}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Participant>();
		}

		public static async Task<Participant> JoinParticipantToRoomAsync(this HttpClient client, int roomId, string name, bool handRaised = false)
		{
			var participant = new Participant(roomId, name, handRaised);
			var response = await client.PostAsJsonAsync($"api/{Constants.RoomsRoute}/{roomId}/{Constants.ParticipantsRoute}", participant);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Participant>();
		}

		public static async Task<Participant> UpdateParticipantAsync(this HttpClient client, int roomId, Participant participant)
		{
			var response = await client.PostAsJsonAsync($"api/{Constants.ParticipantsRoute}/{roomId}", participant);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Participant>();
		}


		public static async Task<Participant> DeleteParticipantAsync(this HttpClient client, int participantId)
		{
			var response = await client.DeleteAsync($"api/{Constants.ParticipantsRoute}/{participantId}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsAsync<Participant>();
		}
	}
}
