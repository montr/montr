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

const PageRegisterCompany = React.lazy(() => import("./components/page-register-company"));

AppRouteRegistry.add(Layout.public, [
	{ path: Patterns.registerCompany, element: <PageRegisterCompany /> },
]);
