using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Montr.Tendr.Tests.Misc
{
	[TestClass, Ignore]
	public class AuthTests
	{
		[TestMethod]
		public async Task CallApiUsingClientCredentials()
		{
			var client = new HttpClient();

			// discover endpoints from metadata
			// https://identitymodel.readthedocs.io/en/latest/client/discovery.html
			var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
			{
				Address = "http://idx.montr.io:5050",
				Policy =
				{
					RequireHttps = false
				}
			});

			Assert.IsTrue(disco.IsError == false, disco.Error);

			Console.WriteLine("DiscoveryResponse:\n" + disco.Json + "\n");

			// request token
			var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
			{
				Address = disco.TokenEndpoint,

				ClientId = "tendr",
				ClientSecret = "secret",
				Scope = "tendr"
			});

			Assert.IsTrue(tokenResponse.IsError == false, tokenResponse.Error);

			Console.WriteLine("TokenResponse:\n" + tokenResponse.Json + "\n");

			// call api
			client.SetBearerToken(tokenResponse.AccessToken);

			var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(
				"{\"sortColumn\":\"id\",\"sortOrder\":\"descending\"}"));
			httpContent.Headers.Add("Content-Type", "application/json");

			var response = await client.PostAsync("http://app.tendr.montr.io:5000/api/events/load", httpContent);

			Assert.IsTrue(response.IsSuccessStatusCode, "Response status code: " + response.StatusCode);
			
			var content = await response.Content.ReadAsStringAsync();
			Console.WriteLine("ApiResponse:\n" + JObject.Parse(content) + "\n");
		}
	}
}
