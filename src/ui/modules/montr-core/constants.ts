declare var
	AUTHORITY_APP_URL: string,
	APP_URL: string,
	COOKIE_DOMAIN: string;

export const Constants = {
	cookieDomain: COOKIE_DOMAIN,
	// todo: move or rename
	cookieName: "current_company_uid",
	authorityURL: AUTHORITY_APP_URL,
	publicURL: APP_URL,
	apiURL: APP_URL + "/api",

	returnUrlParam: "ReturnUrl",
	returnUrlParamLower: "returnUrl", // todo: why case differ from Constants.returnUrl?

	defaultPageSize: 10
};

export const Layout = {
	auth: "auth",
	public: "public",
	private: "private"
};
