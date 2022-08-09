import { Layout } from "@montr-core/.";
import { AppRouteRegistry } from "@montr-core/services";
import React from "react";

export const Api = {
	companyRegistrationRequestList: "/companyRegistrationRequest/search",
	companyRegistrationRequestCreate: "/companyRegistrationRequest/create",
	companyRegistrationRequestDelete: "/companyRegistrationRequest/delete"
};

export const Patterns = {
	registration: "/registration/",
};

AppRouteRegistry.add(Layout.public, [
	{ path: Patterns.registration, component: React.lazy(() => import("./components/page-registration")) },
]);
