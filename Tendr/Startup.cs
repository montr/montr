using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityServer4.AccessTokenValidation;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tendr.Services;

namespace Tendr
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;

			DataConnection.DefaultSettings = new DbSettings(Configuration);
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.Formatting = Formatting.None;
					options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(true));
					options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				});

			// for api login
			/*services
				.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(options =>
				{
					options.Authority = "http://idx.local:5050";
					options.RequireHttpsMetadata = false;

					options.ApiName = "tendr";
				});*/

			// https://github.com/IdentityServer/IdentityServer3/issues/487
			// leastprivilege commented on Nov 4, 2014
			// Just in general - i would recommend separating web app and api
			// - that way you don't run into the whole cookie vs token isolation issue and the related configuration complexity.

			// https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2836
			// Support both openId and bearer token of the identity server？

			// for user login
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			var openIdOptions = Configuration.GetSection("OpenId").Get<OpenIdOptions>();

			services
				.AddAuthentication(options =>
				{
					options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
				})
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
				{
					options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

					options.Authority = openIdOptions.Authority;
					options.RequireHttpsMetadata = false;

					options.ClientId = openIdOptions.ClientId;
					options.ClientSecret = openIdOptions.ClientSecret;
					options.ResponseType = "code id_token"; // code id_token token

					options.SaveTokens = true;
					options.GetClaimsFromUserInfoEndpoint = true;

					options.Scope.Add("tendr");
					options.Scope.Add("offline_access");
					// options.ClaimActions.MapJsonKey("website", "website");
				});

			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			/*if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else*/
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			// app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			/*app.UseSpa(x =>
			{
				x.Options.DefaultPage = "/";
			});*/

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}

	public class OpenIdOptions
	{
		public string Authority { get; set; }

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }
	}

	public class ConnectionStringSettings : IConnectionStringSettings
	{
		public string ConnectionString { get; set; }

		public string Name { get; set; }

		public string ProviderName { get; set; }

		public bool IsGlobal => false;
	}

	public class DbSettings : ILinqToDBSettings
	{
		private readonly IConfiguration _configuration;
		public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

		public string DefaultConfiguration => "SqlServer";
		public string DefaultDataProvider => "SqlServer";

		public DbSettings(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IEnumerable<IConnectionStringSettings> ConnectionStrings
		{
			get
			{
				var config = _configuration.GetSection("ConnectionString")
					.Get<ConnectionStringSettings>();

				yield return config;
			}
		}
	}
}
