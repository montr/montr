import { Layout } from "@montr-core/constants";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services/";
import React from "react";
import { generatePath } from "react-router";

export const Locale = {
	Namespace: "tendr"
};

export const Api = {
	eventMetadataView: "/eventMetadata/view",

	eventTemplateList: "/eventTemplate/list",

	eventsList: "/events/list",
	eventsGet: "/events/get",
	eventsInsert: "/events/insert",
	eventsUpdate: "/events/update",
	eventsPublish: "/events/publish",
	eventsCancel: "/events/cancel",

	eventInvitationList: "/invitation/list",
	eventInvitationGet: "/invitation/get",
	eventInvitationInsert: "/invitation/insert",
	eventInvitationUpdate: "/invitation/update",
	eventInvitationDelete: "/invitation/delete"
};

export const Patterns = {
	searchEvents: "/events",
	addEvent: "/events/new",
	editEvent: "/events/edit/:uid",
	editEventTab: "/events/edit/:uid/:tabKey",
};

export const EntityTypeCode = {
	event: "event"
};

export const Views = {
	eventInvitationList: "Event/Invitation/List"
};

export const RouteBuilder = {
	editEvent: (uid: string, tabKey = "info"): string => {
		return generatePath(Patterns.editEventTab, { uid, tabKey });
	},
};

const PageSearchEvents = React.lazy(() => import("./components/page-search-events"));
const PageSelectEventTemplate = React.lazy(() => import("./components/page-select-event-template"));
const PageEditEvent = React.lazy(() => import("./components/page-edit-event"));

AppRouteRegistry.add(Layout.private, [
	{ path: Patterns.searchEvents, element: <PageSearchEvents /> },
	{ path: Patterns.addEvent, element: <PageSelectEventTemplate /> },
	{ path: Patterns.editEvent, element: <PageEditEvent /> }
]);

ComponentRegistry.add([
	{ path: "@montr-tendr/components/tab-edit-event", component: React.lazy(() => import("./components/tab-edit-event")) },
	{ path: "@montr-tendr/components/tab-edit-invitations", component: React.lazy(() => import("./components/tab-edit-invitations")) }
]);
