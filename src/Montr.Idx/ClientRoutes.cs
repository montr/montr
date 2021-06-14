namespace Montr.Idx
{
	// todo: move to impl?
	public static class ClientRoutes
	{
		public static readonly string Login = "/account/login";

		public static readonly string Logout = "/account/logout";

		public static readonly string Register = "/account/register";

		public static readonly string RegisterConfirmation = "/account/register-confirmation";

		public static readonly string ConfirmEmail = "/account/confirm-email";

		public static readonly string ConfirmEmailChange = "/account/confirm-email-change";

		public static readonly string ResetPassword = "/account/reset-password";

		public static readonly string ExternalLogin = "/account/external-login";

		public static readonly string LinkLogin = "/profile/external-login/link";
	}
}
