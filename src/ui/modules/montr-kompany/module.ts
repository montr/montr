import { Layout } from "@montr-core/constants";
import { AppRouteRegistry } from "@montr-core/services";
import React from "react";

export const Api = {
	companyList: "/userCompany/list",
	companyCreate: "/userCompany/create",

	companyMetadataView: "/companyMetadata/view",
};

export const Patterns = {
	registerCompany: "/register/company/"
};

export const Views = {
};

AppRouteRegistry.add(Layout.public, [
	{ path: Patterns.registerCompany, component: React.lazy(() => import("./components/page-register-company")) },
]);
