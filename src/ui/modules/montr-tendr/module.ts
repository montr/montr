import React from "react";
import { generatePath } from "react-router";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services/";
import { Constants } from "@montr-core/constants";

export const Locale = {
	Namespace: "tendr"
};

export const Api = {
	eventInvitationList: `${Constants.apiURL}/invitation/list/`
};

export const Patterns = {
	searchEvents: "/events/",
	addEvent: "/events/new",
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
	{ path: Patterns.searchEvents, exact: true, component: React.lazy(() => import("./components/page-search-events")) },
	{ path: Patterns.addEvent, exact: true, component: React.lazy(() => import("./components/page-select-event-template")) },
	{ path: Patterns.editEvent, exact: true, component: React.lazy(() => import("./components/page-edit-event")) }
]);

ComponentRegistry.add([
	{ path: "panes/private/EditEventPane", component: React.lazy(() => import("./components/tab-edit-event")) },
	{ path: "panes/private/InvitationPane", component: React.lazy(() => import("./components/tab-edit-invitations")) }
]);
