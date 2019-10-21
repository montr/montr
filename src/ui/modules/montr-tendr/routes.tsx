import { generatePath } from "react-router";
import { Registration } from "./pages/public";
import { Registration as CompanyRegistration } from "@montr-kompany/pages/public/";
import { SearchEvents, SelectEventTemplate, EditEvent } from "./pages/private";
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
	{ path: "/register/", layout: "public", exact: true, component: Registration },
	{ path: "/register/company/", layout: "public", exact: true, component: CompanyRegistration },

	{ path: "/events/", exact: true, component: SearchEvents },
	{ path: "/events/new", exact: true, component: SelectEventTemplate },
	{ path: Patterns.editEvent, exact: true, component: EditEvent }
];

AppRouteRegistry.add(Routes);
