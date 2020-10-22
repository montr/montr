import React from "react";
import { generatePath } from "react-router";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services/";
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
	{ path: "/register/", layout: Layout.public, exact: true, component: React.lazy(() => import("./components/page-registration")) },
	{ path: "/register/company/", layout: Layout.public, exact: true, component: React.lazy(() => import("@montr-kompany/components/page-register")) },
	{ path: "/events/", exact: true, component: React.lazy(() => import("./components/page-search-events")) },
	{ path: "/events/new", exact: true, component: React.lazy(() => import("./components/page-select-event-template")) },
	{ path: Patterns.editEvent, exact: true, component: React.lazy(() => import("./components/page-edit-event")) }
]);

ComponentRegistry.add([
	{ path: "panes/private/EditEventPane", component: React.lazy(() => import("./components/tab-edit-event")) },
	{ path: "panes/private/InvitationPane", component: React.lazy(() => import("./components/tab-edit-invitations")) }
]);
