import React from "react";
import { generatePath } from "react-router";
import { AppRouteRegistry } from "@montr-core/services/";
import { Layout } from "@montr-core/constants";

export const Patterns = {
	editEvent: "/events/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editEvent: (uid: string, tabKey?: string) => {
		return generatePath(Patterns.editEvent, { uid, tabKey });
	},
};

AppRouteRegistry.add([
	{ path: "/register/", layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/public/registration")) },
	{ path: "/register/company/", layout: Layout.public, exact: true, component: React.lazy(() => import("@montr-kompany/pages/register")) },
	{ path: "/events/", exact: true, component: React.lazy(() => import("./pages/private/search-events")) },
	{ path: "/events/new", exact: true, component: React.lazy(() => import("./pages/private/select-event-template")) },
	{ path: Patterns.editEvent, exact: true, component: React.lazy(() => import("./pages/private/edit-event")) }
]);
