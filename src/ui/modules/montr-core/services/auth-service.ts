import { Log, SignoutResponse, User, UserManager, UserManagerSettings } from "oidc-client";
import { Constants } from "../constants";
import { NavigationService } from "./navigation-service";

class AuthConstants {
	public static authority = Constants.authorityURL;
	public static clientId = "ui";
	public static clientRoot = window.location.origin;

	public static RedirectUri = "/signin-oidc";
	public static SilentRedirectUri = "/silent-renew-oidc";
	public static PostLogoutRedirectUri = "/signout-callback-oidc";
}

// todo: move to idx?
export class AuthService {
	private static instance: AuthService;

	private readonly _userManager!: UserManager;
	private readonly navigator: NavigationService = new NavigationService();

	constructor() {
		if (AuthService.instance) {
			return AuthService.instance;
		}

		Log.logger = console;
		Log.level = Log.WARN;

		// todo: normal check, to prevent cancelled .well-known/openid-configuration requests in iframe
		const runTasks = true; // (window.frameElement == null);

		// https://github.com/IdentityModel/oidc-client-js/wiki
		// https://openid.net/specs/openid-connect-core-1_0.html#Authentication
		const settings: UserManagerSettings = {
			authority: AuthConstants.authority,
			client_id: AuthConstants.clientId,

			redirect_uri: AuthConstants.clientRoot + AuthConstants.RedirectUri,
			silent_redirect_uri: AuthConstants.clientRoot + AuthConstants.SilentRedirectUri,
			post_logout_redirect_uri: AuthConstants.clientRoot + AuthConstants.PostLogoutRedirectUri,

			response_type: "id_token token",
			scope: "openid profile email",
			automaticSilentRenew: runTasks,
			monitorSession: runTasks
		};

		this._userManager = new UserManager(settings);

		// todo: use logger here and below

		/* this._userManager.events.addAccessTokenExpired((...args: any[]) => {
			console.log("AccessTokenExpired", window.frameElement, args);
		});
		this._userManager.events.addAccessTokenExpiring((...args: any[]) => {
			console.log("AccessTokenExpiring", window.frameElement, args);
		});
		this._userManager.events.addSilentRenewError((...args: any[]) => {
			console.log("SilentRenewError", window.frameElement, args);
		});
		this._userManager.events.addUserLoaded((...args: any[]) => {
			console.log("UserLoaded", window.frameElement, args);
		});
		this._userManager.events.addUserSessionChanged((...args: any[]) => {
			console.log("UserSessionChanged", window.frameElement, args);
		});
		this._userManager.events.addUserSignedOut((...args: any[]) => {
			console.log("UserSignedOut", window.frameElement, args);
		});
		this._userManager.events.addUserUnloaded((...args: any[]) => {
			console.log("UserUnloaded", window.frameElement, args);
		}); */

		AuthService.instance = this;
	}

	public isCallback(): boolean {
		const url = this.navigator.getUrl();

		return (
			url.indexOf(AuthConstants.RedirectUri) !== -1 ||
			url.indexOf(AuthConstants.SilentRedirectUri) !== -1 ||
			url.indexOf(AuthConstants.PostLogoutRedirectUri) !== -1
		);
	}

	public processCallback(): void {
		const url = this.navigator.getUrl();

		// console.log("processCallback()", window.frameElement, url);

		if (url.indexOf(AuthConstants.RedirectUri) !== -1) {
			this._userManager.signinRedirectCallback(url)
				.then((user: User) => {
					this.signinRedirectCallback(user);
				}).catch(function (e) {
					console.error(e);
				});
		} else if (url.indexOf(AuthConstants.SilentRedirectUri) !== -1) {
			this._userManager.signinSilentCallback(url)
				.catch(function (e) {
					console.error(e);
				});
		} else if (url.indexOf(AuthConstants.PostLogoutRedirectUri) !== -1) {
			this._userManager.signoutRedirectCallback(url)
				.then((value: SignoutResponse) => {
					this.signoutRedirectCallback(value);
				}).catch(function (e) {
					console.error(e);
				});
		}
	}

	get userManager(): UserManager {
		return this._userManager;
	}

	public getUser(): Promise<User> {
		return this._userManager.getUser();
	}

	public login(): Promise<any> {
		const args = this.getRedirectArgs();

		// console.log("login()", args);

		return this._userManager.signinRedirect(args);
	}

	public loginSilent(): Promise<User> {
		// console.log("loginSilent()");

		return this._userManager.signinSilent();
	}

	public logout(): Promise<any> {
		const args = this.getRedirectArgs();

		// console.log("logout()", args);

		return this._userManager.signoutRedirect(args);
	}

	private getRedirectArgs() {
		return {
			state: {
				return_uri: this.navigator.getUrl()
			}
		};
	}

	private signinRedirectCallback(value: User) {
		let return_uri;
		if (value && value.state) {
			return_uri = value.state.return_uri;
		}

		// console.log("signinRedirectCallback()", value);

		this.navigator.navigate(return_uri || "/");
	}

	private signoutRedirectCallback(value: SignoutResponse) {
		let return_uri;
		if (value && value.state) {
			return_uri = value.state.return_uri;
		}

		// console.log("signoutRedirectCallback()", value);

		this.navigator.navigate(return_uri || "/");
	}

	public onAuthenticated(callback: (user: User) => void): void {
		return this._userManager.events.addUserLoaded((user: User) => {
			callback(user);
		});
	}

	public addUserSignedOut(callback: () => void): void {
		return this._userManager.events.addUserSignedOut(() => {
			callback();
		});
	}
}
