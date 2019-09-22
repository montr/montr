declare var
	AUTHORITY_APP_URL: string,
	PUBLIC_APP_URL: string,
	PRIVATE_APP_URL: string,
	COOKIE_DOMAIN: string;

export const Constants = {
	cookieDomain: COOKIE_DOMAIN,
	// todo: move or rename
	cookieName: "current_company_uid",
	authorityURL: AUTHORITY_APP_URL,
	publicURL: PUBLIC_APP_URL,
	privateURL: PRIVATE_APP_URL,
	apiURL: PUBLIC_APP_URL + "/api",
	returnUrlParam: "return_url",
	defaultPageSize: 10
};
