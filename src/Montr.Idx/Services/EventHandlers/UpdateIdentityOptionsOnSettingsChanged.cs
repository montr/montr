using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Montr.Core.Services;
using Montr.Idx.Entities;
using Montr.Settings.Events;

namespace Montr.Idx.Services.EventHandlers
{
	/// <summary>
	/// UserManager takes IOptions of IdentityOptions in constructor and values not reloaded,
	/// so we need to set these options to UserManager after they are changed.
	/// </summary>
	public class UpdateIdentityOptionsOnSettingsChanged : INotificationHandler<SettingsChanged>
	{
		private readonly UserManager<DbUser> _userManager;
		private readonly IOptionsMonitor<SignInSettings> _identitySignInSettings;
		private readonly IOptionsMonitor<PasswordSettings> _identityPasswordSettings;
		private readonly IOptionsMonitor<LockoutSettings> _identityLockoutSettings;

		public UpdateIdentityOptionsOnSettingsChanged(UserManager<DbUser> userManager,
			IOptionsMonitor<SignInSettings> identitySignInSettings,
			IOptionsMonitor<PasswordSettings> identityPasswordSettings,
			IOptionsMonitor<LockoutSettings> identityLockoutSettings)
		{
			_userManager = userManager;
			_identitySignInSettings = identitySignInSettings;
			_identityPasswordSettings = identityPasswordSettings;
			_identityLockoutSettings = identityLockoutSettings;
		}

		public Task Handle(SettingsChanged notification, CancellationToken cancellationToken)
		{
			var keys = notification.Values.Select(x => x.Item1).ToList();

			if (keys.Exists(key => key.StartsWith(OptionsUtils.GetOptionsSectionKey<SignInSettings>())))
			{
				_identitySignInSettings.CurrentValue.MapTo(_userManager.Options.SignIn);
			}

			if (keys.Exists(key => key.StartsWith(OptionsUtils.GetOptionsSectionKey<PasswordSettings>())))
			{
				_identityPasswordSettings.CurrentValue.MapTo(_userManager.Options.Password);
			}

			if (keys.Exists(key => key.StartsWith(OptionsUtils.GetOptionsSectionKey<LockoutSettings>())))
			{
				_identityLockoutSettings.CurrentValue.MapTo(_userManager.Options.Lockout);
			}

			return Task.CompletedTask;
		}
	}
}
