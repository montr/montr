import React from "react";
import { IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/";

export const Patterns = {
	confirmEmail: "/account/confirm-email/:userId/:code"
};

export const Routes: IRoute[] = [
	{ path: "/account/login", layout: "public", exact: true, component: React.lazy(() => import("./pages/login")) },
	{ path: "/account/register", layout: "public", exact: true, component: React.lazy(() => import("./pages/register")) },
	{ path: Patterns.confirmEmail, layout: "public", exact: true, component: React.lazy(() => import("./pages/confirm-email")) },
	{ path: "/account/forgot-password", layout: "public", exact: true, component: React.lazy(() => import("./pages/forgot-password")) },
];

AppRouteRegistry.add(Routes);
