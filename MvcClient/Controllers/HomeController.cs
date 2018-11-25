using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using Newtonsoft.Json.Linq;

namespace MvcClient.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Secure()
		{
			ViewData["Message"] = "Secure page.";

			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public async Task<IActionResult> CallApiUsingClientCredentials()
		{
			var tokenClient = new TokenClient("http://idx.local:5050/connect/token", "tendr", "secret");
			var tokenResponse = await tokenClient.RequestClientCredentialsAsync("tendr");

			var client = new HttpClient();
			client.SetBearerToken(tokenResponse.AccessToken);
			
			var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(
				"{\"sortColumn\":\"id\",\"sortOrder\":\"descending\"}"));
			httpContent.Headers.Add("Content-Type", "application/json");

			var response = await client.PostAsync("http://app.tendr.local:5000/api/events/load", httpContent);

			var content = await response.Content.ReadAsStringAsync();

			ViewBag.Json = JObject.Parse(content).ToString();
			return Content(ViewBag.Json);
		}

		public async Task<IActionResult> CallApiUsingUserAccessToken()
		{
			var accessToken = await HttpContext.GetTokenAsync("access_token");

			var client = new HttpClient();
			client.SetBearerToken(accessToken);
			
			var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(
				"{\"sortColumn\":\"id\",\"sortOrder\":\"descending\"}"));
			httpContent.Headers.Add("Content-Type", "application/json");

			var response = await client.PostAsync("http://app.tendr.local:5000/api/events/load", httpContent);

			var content = await response.Content.ReadAsStringAsync();

			ViewBag.Json = JObject.Parse(content).ToString();
			return Content(ViewBag.Json);
		}
	}
}
