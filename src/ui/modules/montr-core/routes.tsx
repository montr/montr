import React from "react";
import { IRoute } from "./models";
import { AppRouteRegistry } from "./services/";

export const Routes: IRoute[] = [
	{ path: "/", layout: "public", exact: true, component: React.lazy(() => import("./pages/home")) },
	{ path: "/login", layout: "public", exact: true, component: React.lazy(() => import("./pages/login")) },

	{ path: "/dashboard/", exact: true, component: React.lazy(() => import("./pages/dashboard")) },
	{ path: "/locales/", exact: true, component: React.lazy(() => import("./pages/search-locale-string")) },
	{ path: "/settings/", exact: true, component: React.lazy(() => import("./pages/settings")) },
];

AppRouteRegistry.add(Routes);
