declare var
	KOMPANY_APP_URL: string,
	COOKIE_DOMAIN: string;

export const Constants = {
	cookieDomain: COOKIE_DOMAIN,
	cookieName: "current_company_uid",
	baseURL: KOMPANY_APP_URL,
	apiURL: KOMPANY_APP_URL + "/api",
	returnUrlParam: "return_url"
};
