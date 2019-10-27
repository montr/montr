import React from "react";
import { IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/";

export const Patterns = {
	register: "/account/register",
	confirmEmail: "/account/confirm-email/:userId/:code",
	login: "/account/login",
	logout: "/account/logout",
	forgotPassword: "/account/forgot-password",
	resetPassword: "/account/reset-password/:code",
};

export const Routes: IRoute[] = [
	{ path: Patterns.register, layout: "public", exact: true, component: React.lazy(() => import("./pages/register")) },
	{ path: Patterns.confirmEmail, layout: "public", exact: true, component: React.lazy(() => import("./pages/confirm-email")) },
	{ path: Patterns.login, layout: "public", exact: true, component: React.lazy(() => import("./pages/login")) },
	{ path: Patterns.logout, layout: "public", exact: true, component: React.lazy(() => import("./pages/logout")) },
	{ path: Patterns.forgotPassword, layout: "public", exact: true, component: React.lazy(() => import("./pages/forgot-password")) },
	{ path: Patterns.resetPassword, layout: "public", exact: true, component: React.lazy(() => import("./pages/reset-password")) },
];

AppRouteRegistry.add(Routes);
