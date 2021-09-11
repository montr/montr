import { Constants, Layout } from "@montr-core/.";
import { AppRouteRegistry } from "@montr-core/services";
import React from "react";

export const Api = {
    companyRegistrationRequestList: `${Constants.apiURL}/companyRegistrationRequest/search`,
    companyRegistrationRequestCreate: `${Constants.apiURL}/companyRegistrationRequest/create`,
    companyRegistrationRequestDelete: `${Constants.apiURL}/companyRegistrationRequest/delete`
};

export const Patterns = {
    registration: "/registration/",
};

AppRouteRegistry.add([
    { path: Patterns.registration, layout: Layout.public, exact: true, component: React.lazy(() => import("./components/page-registration")) },
]);
