import { Log, User, UserManager } from "oidc-client";

class Constants {
	public static authority = "http://idx.montr.io:5050";
	public static clientId = "ui";
	public static clientRoot = window.location.origin; // "http://kompany.montr.io:5010";
	public static clientScope = "openid profile"; // email tendr

	// public static apiRoot = "https://demo.identityserver.io/api/";
}

export class AuthService {
	static instance: AuthService;

	public userManager: UserManager;

	constructor() {
		if (AuthService.instance) {
			return AuthService.instance;
		}

		const settings = {
			authority: Constants.authority,
			client_id: Constants.clientId,
			redirect_uri: `${Constants.clientRoot}/signin-oidc`,
			silent_redirect_uri: `${Constants.clientRoot}/signin-oidc`,
			// silent_redirect_uri: `${Constants.clientRoot}/silent-renew.html`,
			// tslint:disable-next-line:object-literal-sort-keys
			// post_logout_redirect_uri: `${Constants.clientRoot}/signout-callback-oidc`,
			post_logout_redirect_uri: `${Constants.clientRoot}`,
			// revokeAccessTokenOnSignout: true,
			response_type: "id_token token",
			// response_type: "code",
			scope: Constants.clientScope,
			automaticSilentRenew: true,
		};

		this.userManager = new UserManager(settings);
		this.userManager.events.addUserLoaded((user) => {
			console.log("AUTH SERVICE --- USER LOADED", window.frameElement, user);
		});

		Log.logger = console;
		Log.level = Log.INFO;

		AuthService.instance = this;
	}

	public getUser(): Promise<User> {
		return this.userManager.getUser();
	}

	public login(): Promise<void> {
		return this.userManager.signinRedirect();
	}

	public loginSilent(): Promise<User> {
		return this.userManager.signinSilent();
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

	public addUserLoaded(callback: (...ev: any[]) => void) {
		this.userManager.events.addUserLoaded(callback);
	}
}
