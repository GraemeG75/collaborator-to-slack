using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace CollaboratorToSlack
{
	public class Slack
	{
		public Slack()
		{

		}

		private static void PostMessage(string channelId, string reviewId, string blocks)
		{

			IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>>
			{
				new("channel", channelId),
				new("text", $"Review #{reviewId} Updated"),
				new("blocks", blocks)
			};

			CallApi(HttpMethods.Post, "api/chat.postMessage", new FormUrlEncodedContent(formData));
		}

		private static string GetUserIdFromEmail(string email)
		{

			HttpResponseMessage response = CallApi(HttpMethods.Get, $"api/users.lookupByEmail?email={email}");

			if (response == null)
			{
				return string.Empty;
			}

			JObject responseObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
			if (responseObject["ok"] == null)
			{
				return string.Empty;
			}

			if (!responseObject["ok"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				return string.Empty;
			}

			return responseObject["user"]?["id"] != null ? responseObject["user"]["id"].ToString() : string.Empty;
		}


		private static HttpResponseMessage CallApi(HttpMethods method, string endPoint, HttpContent formData = null)
		{
			using HttpClient httpClient = new()
			{
				BaseAddress = new Uri("https://slack.com/")
			};
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

			HttpResponseMessage response = method switch
			{
				HttpMethods.Get => httpClient.GetAsync(endPoint).Result,
				HttpMethods.Post => httpClient.PostAsync(endPoint, formData ?? new StringContent("")).Result,
				_ => null
			};

			if (response != null)
			{
				response.EnsureSuccessStatusCode();
			}

			return response;
		}
	}
}
