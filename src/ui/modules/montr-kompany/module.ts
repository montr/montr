import React from "react";
import { Constants, Layout } from "@montr-core/constants";
import { AppRouteRegistry } from "@montr-core/services";

export const Api = {
	companyRegistrationRequestList: `${Constants.apiURL}/companyRegistration/requests`,
	companyRegistrationRequestCreate: `${Constants.apiURL}/companyRegistration/createRequest`,

	companyList: `${Constants.apiURL}/userCompany/list`,
	companyCreate: `${Constants.apiURL}/userCompany/create`,
};

export const Patterns = {
	registration: "/register/",
	registerCompany: "/register/company/"
};

export const Views = {
};

AppRouteRegistry.add([
	{ path: Patterns.registration, layout: Layout.public, exact: true, component: React.lazy(() => import("./components/page-registration")) },
	{ path: Patterns.registerCompany, layout: Layout.public, exact: true, component: React.lazy(() => import("./components/page-register-company")) },
]);
