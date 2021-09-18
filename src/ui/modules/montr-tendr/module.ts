import { Constants } from "@montr-core/constants";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services/";
import React from "react";
import { generatePath } from "react-router";

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

export const EntityTypeCode = {
	event: "event"
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
	{ path: "@montr-tendr/components/tab-edit-event", component: React.lazy(() => import("./components/tab-edit-event")) },
	{ path: "@montr-tendr/components/tab-edit-invitations", component: React.lazy(() => import("./components/tab-edit-invitations")) }
]);
