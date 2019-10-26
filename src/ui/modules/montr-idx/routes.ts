import React from "react";
import { IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/";

export const Routes: IRoute[] = [
	{ path: "/account/login", layout: "public", exact: true, component: React.lazy(() => import("./pages/login")) },
	{ path: "/account/register", layout: "public", exact: true, component: React.lazy(() => import("./pages/register")) },
];

AppRouteRegistry.add(Routes);
