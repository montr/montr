import React from "react";
import { generatePath } from "react-router";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services/";
import { Constants, Layout } from "@montr-core/constants";

export const Locale = {
	Namespace: "tendr"
};

export const Api = {
	eventInvitationList: `${Constants.apiURL}/invitation/list/`
};

export const Patterns = {
	editEvent: "/events/edit/:uid/:tabKey?",
};

export const Views = {
	eventInvitationList: "Event/Invitation/List"
};

export const RouteBuilder = {
	editEvent: (uid: string, tabKey?: string): string => {
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
