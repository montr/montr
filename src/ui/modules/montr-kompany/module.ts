import React from "react";
import { Constants } from "@montr-core/constants";
import { AppRouteRegistry } from "@montr-core/services";

export const Api = {
	companyList: `${Constants.apiURL}/company/list`,
};

export const Patterns = {
	searchUsers: "/companies",
};

export const Views = {
	gridSearchCompanies: "CompanySearch/Grid",
};

AppRouteRegistry.add([
	{ path: Patterns.searchUsers, exact: true, component: React.lazy(() => import("./components/page-search-companies")) },
]);
