import { Log, User, UserManager } from "oidc-client";
import { message } from "antd";
import { NavigationService } from "./navigation-service";

class Constants {
	public static authority = "http://idx.montr.io:5050";
	public static clientId = "ui";
	public static clientRoot = window.location.origin;
	public static clientScope = "openid profile email tendr";

	public static RedirectUri = "/signin-oidc";
	public static SilentRedirectUri = "/silent-renew-oidc";
	public static PostLogoutRedirectUri = "/signout-callback-oidc";
}

export class AuthService {
	static instance: AuthService;

	private _userManager: UserManager;
	private _navigator: NavigationService;

	constructor() {
		if (AuthService.instance) {
			return AuthService.instance;
		}

		Log.logger = console;
		Log.level = Log.WARN;

		// todo: normal check, to prevent cancelled .well-known/openid-configuration requests in iframe
		const runTasks = (window.frameElement == null)

		// https://github.com/IdentityModel/oidc-client-js/wiki
		// https://openid.net/specs/openid-connect-core-1_0.html#Authentication
		const settings = {
			authority: Constants.authority,
			client_id: Constants.clientId,
			redirect_uri: Constants.clientRoot + Constants.RedirectUri,
			silent_redirect_uri: Constants.clientRoot + Constants.SilentRedirectUri,
			post_logout_redirect_uri: Constants.clientRoot + Constants.PostLogoutRedirectUri,
			// revokeAccessTokenOnSignout: true,
			// response_type: "id_token token",
			response_type: "code",
			scope: Constants.clientScope,
			automaticSilentRenew: runTasks,
			monitorSession: runTasks
		};

		this._userManager = new UserManager(settings);
		this._navigator = new NavigationService();

		// todo: use logger here and below
		this._userManager.events.addAccessTokenExpired((...args: any[]) => {
			// console.log("AccessTokenExpired", window.frameElement, args);
			message.error("AccessTokenExpired");
		});
		this._userManager.events.addAccessTokenExpiring((...args: any[]) => {
			// console.log("AccessTokenExpiring", window.frameElement, args);
			message.warning("AccessTokenExpiring");
		});
		this._userManager.events.addSilentRenewError((...args: any[]) => {
			// console.log("SilentRenewError", window.frameElement, args);
			message.error("SilentRenewError");
		});
		this._userManager.events.addUserLoaded((...args: any[]) => {
			// console.log("UserLoaded", window.frameElement, args);
			message.info("UserLoaded");
		});
		this._userManager.events.addUserSessionChanged((...args: any[]) => {
			// console.log("UserSessionChanged", window.frameElement, args);
			message.info("UserSessionChanged");
		});
		this._userManager.events.addUserSignedOut((...args: any[]) => {
			// console.log("UserSignedOut", window.frameElement, args);
			message.info("UserSignedOut");
		});
		this._userManager.events.addUserUnloaded((...args: any[]) => {
			// console.log("UserUnloaded", window.frameElement, args);
			message.info("UserUnloaded");
		});

		AuthService.instance = this;
	}

	public isCallback() {
		const url = this._navigator.getUrl();

		return (
			url.indexOf(Constants.RedirectUri) != -1 ||
			url.indexOf(Constants.SilentRedirectUri) != -1 ||
			url.indexOf(Constants.PostLogoutRedirectUri) != -1
		);
	}

	public processCallback() {
		const url = this._navigator.getUrl();

		if (url.indexOf(Constants.RedirectUri) != -1) {
			this._userManager.signinRedirectCallback(url)
				.then((user: User) => {
					this.redirectCallback(user);
				}).catch(function (e) {
					console.error(e);
				});
		} else if (url.indexOf(Constants.SilentRedirectUri) != -1) {
			this._userManager.signinSilentCallback(url)
				.catch(function (e) {
					console.error(e);
				});
		} else if (url.indexOf(Constants.PostLogoutRedirectUri) != -1) {
			this._userManager.signoutRedirectCallback(url)
				.then((user: User) => {
					this.redirectCallback(user);
				}).catch(function (e) {
					console.error(e);
				});
		}
	}

	public getUser(): Promise<User> {
		return this._userManager.getUser();
	}

	public login(): Promise<any> {
		const args = this.getRedirectArgs();
		return this._userManager.signinRedirect(args);
	}

	public loginSilent(): Promise<User> {
		return this._userManager.signinSilent();
	}

	public logout(): Promise<any> {
		const args = this.getRedirectArgs();
		return this._userManager.signoutRedirect(args);
	}

	private getRedirectArgs() {
		return {
			state: {
				return_uri: this._navigator.getUrl()
			}
		};
	}

	private redirectCallback(user: User) {
		let return_uri;
		if (user && user.state) {
			return_uri = user.state.return_uri
		}
		this._navigator.navigate(return_uri || "/");
	}

	public onAuthenticated(callback: (user: User) => void): void {
		// todo: add PR to rename addUserLoaded -> addAuthenticated
		return this._userManager.events.addUserLoaded((user: User) => {
			callback(user);
		})
	}
}
