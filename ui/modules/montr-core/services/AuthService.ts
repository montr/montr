import { Log, User, UserManager } from "oidc-client";

class Constants {
	public static authority = "http://idx.montr.io:5050";
	public static clientId = "ui";
	public static clientRoot = "http://kompany.montr.io:5010";
	public static clientScope = "openid profile tendr"; // email api

	// public static apiRoot = "https://demo.identityserver.io/api/";
}

export class AuthService {
	public userManager: UserManager;

	constructor() {
		const settings = {
			authority: Constants.authority,
			client_id: Constants.clientId,
			redirect_uri: `${Constants.clientRoot}/signin-oidc`,
			silent_redirect_uri: `${Constants.clientRoot}/silent-renew.html`,
			// tslint:disable-next-line:object-literal-sort-keys
			post_logout_redirect_uri: `${Constants.clientRoot}/signout-callback-oidc`,
			response_type: "id_token token",
			scope: Constants.clientScope
		};
		this.userManager = new UserManager(settings);

		Log.logger = console;
		Log.level = Log.INFO;
	}

	public getUser(): Promise<User> {
		return this.userManager.getUser();
	}

	public login(): Promise<void> {
		return this.userManager.signinRedirect();
	}

	public signinRedirectCallback(): Promise<User> {
		return this.userManager.signinRedirectCallback();
	}

	public renewToken(): Promise<User> {
		return this.userManager.signinSilent();
	}

	public logout(): Promise<void> {
		return this.userManager.signoutRedirect();
	}
}
