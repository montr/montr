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
	returnUrlParam: "return_url",
	defaultPageSize: 10
};

export const Layout = {
	public: "public",
	private: "private"
};
