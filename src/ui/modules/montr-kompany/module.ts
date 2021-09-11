import { Constants, Layout } from "@montr-core/constants";
import { AppRouteRegistry } from "@montr-core/services";
import React from "react";

export const Api = {
	companyList: `${Constants.apiURL}/userCompany/list`,
	companyCreate: `${Constants.apiURL}/userCompany/create`,
};

export const Patterns = {
	registerCompany: "/register/company/"
};

export const Views = {
};

AppRouteRegistry.add([
	{ path: Patterns.registerCompany, layout: Layout.public, exact: true, component: React.lazy(() => import("./components/page-register-company")) },
]);
