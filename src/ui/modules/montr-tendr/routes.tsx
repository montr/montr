import React from "react";
import { generatePath } from "react-router";
import { IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/";

export const Patterns = {
	editEvent: "/events/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editEvent: (uid: string, tabKey?: string) => {
		return generatePath(Patterns.editEvent, { uid, tabKey });
	},
};

export const Routes: IRoute[] = [
	{ path: "/register/", layout: "public", exact: true, component: React.lazy(() => import("./pages/public/registration")) },
	{ path: "/register/company/", layout: "public", exact: true, component: React.lazy(() => import("@montr-kompany/pages/public/register")) },

	{ path: "/events/", exact: true, component: React.lazy(() => import("./pages/private/search-events")) },
	{ path: "/events/new", exact: true, component: React.lazy(() => import("./pages/private/select-event-template")) },
	{ path: Patterns.editEvent, exact: true, component: React.lazy(() => import("./pages/private/edit-event")) }
];

AppRouteRegistry.add(Routes);
