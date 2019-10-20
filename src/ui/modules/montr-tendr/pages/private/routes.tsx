import { generatePath } from "react-router";
import { SearchEvents, SelectEventTemplate, EditEvent } from "../private";
import { IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/app-routes-registry";

export const Patterns = {
	editEvent: "/events/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editEvent: (uid: string, tabKey?: string) => {
		return generatePath(Patterns.editEvent, { uid, tabKey });
	},
};

export const Routes: IRoute[] = [
	{ path: "/events/", exact: true, component: SearchEvents },
	{ path: "/events/new", exact: true, component: SelectEventTemplate },
	{ path: Patterns.editEvent, exact: true, component: EditEvent }
];

AppRouteRegistry.add(Routes);
