import React from "react";
import { generatePath } from "react-router";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services";

export const Patterns = {
	searchProcess: "/processes/",
	editProcess: "/processes/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editProcess: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editProcess, { uid: uid.toString(), tabKey });
	}
};

AppRouteRegistry.add([
	{ path: Patterns.searchProcess, exact: true, component: React.lazy(() => import("./components/page-search-process")) },
]);
