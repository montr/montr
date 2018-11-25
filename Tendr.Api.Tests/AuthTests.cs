using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Tendr.Api.Tests
{
	[TestClass]
	public class AuthTests
	{
		[TestMethod]
		public async Task CallApi()
		{
			// discover endpoints from metadata
			var disco = await DiscoveryClient.GetAsync("http://idx.local:5050");

			Assert.IsTrue(disco.IsError == false, disco.Error);

			// request token
			var tokenClient = new TokenClient(disco.TokenEndpoint, "tendr_client", "tendr_secret");
			var tokenResponse = await tokenClient.RequestClientCredentialsAsync("tendr");

			Assert.IsTrue(tokenResponse.IsError == false, tokenResponse.Error);

			Console.WriteLine(tokenResponse.Json);
			Console.WriteLine("\n\n");

			// call api
			var client = new HttpClient();
			client.SetBearerToken(tokenResponse.AccessToken);

			var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("{\"sortColumn\":\"id\",\"sortOrder\":\"descending\"}"));
			httpContent.Headers.Add("Content-Type", "application/json");

			var response = await client.PostAsync("http://app.tendr.local:5000/api/events/load", httpContent);

			Assert.IsTrue(response.IsSuccessStatusCode, "Response status code: " + response.StatusCode);
			
			var content = await response.Content.ReadAsStringAsync();
			Console.WriteLine(JObject.Parse(content));
		}
	}
}
