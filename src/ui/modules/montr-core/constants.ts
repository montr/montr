// import { AppService } from "./services/app-service";
import { AppOptions } from "./models";

declare var APP_OPTIONS: AppOptions;

if (!APP_OPTIONS.appUrl) APP_OPTIONS.appUrl = window.location.origin;

// new AppService().options().then(options => console.log(options));
// todo: use lib or smth else to load global config from server
// see: https://github.com/morenofa/react-global-configuration
export const Constants = {
	publicURL: APP_OPTIONS.appUrl,
	apiURL: APP_OPTIONS.appUrl + "/api",
	authorityURL: APP_OPTIONS.authorityAppUrl ?? APP_OPTIONS.appUrl,
	cookieDomain: APP_OPTIONS.cookieDomain,
	// todo: move or rename
	cookieName: "current_company_uid",

	returnUrlParam: "ReturnUrl",
	returnUrlParamLower: "returnUrl", // todo: why case differ from Constants.returnUrl?

	defaultPageSize: 10
};

export const Layout = {
	auth: "auth",
	public: "public",
	private: "private"
};
