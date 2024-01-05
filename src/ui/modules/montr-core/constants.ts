// import { AppService } from "./services/app-service";
import { AppOptions, AppState } from "./models";

const getAppOptions = (): AppOptions => {

	const meta = document.querySelector('meta[name="application-name"]');

	return {
		appUrl: meta.attributes.getNamedItem("data-baseUrl")?.value ?? window.location.origin,
		cookieDomain: meta.attributes.getNamedItem("data-cookieDomain")?.value,
		state: meta.attributes.getNamedItem("data-appState")?.value as AppState
	};
};

const options = getAppOptions();

// new AppService().options().then(options => console.log(options));
// todo: use lib or smth else to load global config from server
// see: https://github.com/morenofa/react-global-configuration
export const Constants = {
	publicURL: options.appUrl,
	apiURL: options.appUrl + "/api",
	authorityURL: options.authorityAppUrl ?? options.appUrl,
	cookieDomain: options.cookieDomain,
	appState: options.state,

	// todo: move or rename
	cookieName: "current_company_uid",

	returnUrlParam: "ReturnUrl",
	returnUrlParamLower: "returnUrl", // todo: why case differ from Constants.returnUrl?

	defaultPageSize: 10
};

export const Layout = {
	auth: "auth",
	public: "public",
	private: "private",
	profile: "profile",
	setttings: "settings"
};
