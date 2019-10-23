import { generatePath } from "react-router";
import { Guid, IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/";
import React from "react";

export const Patterns = {
	editClassifierType: "/classifiers/edit/:uid/:tabKey?",

	addClassifier: "/classifiers/:typeCode/add/:parentUid?",
	editClassifier: "/classifiers/:typeCode/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editClassifierType: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editClassifierType, { uid: uid.toString(), tabKey });
	},
	addClassifier: (typeCode: string, parentUid: Guid | string) => {
		return generatePath(Patterns.addClassifier, { typeCode, parentUid: parentUid ? parentUid.toString() : null });
	},
	editClassifier: (typeCode: string, uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editClassifier, { typeCode, uid: uid.toString(), tabKey });
	},
};

export const Routes: IRoute[] = [
	{ path: "/classifiers/", layout: "private", exact: true, component: React.lazy(() => import("./pages/search-classifier-type")) },
	{ path: "/classifiers/add/", layout: "private", exact: true, component: React.lazy(() => import("./pages/edit-classifier-type")) },
	{ path: Patterns.editClassifierType, layout: "private", exact: true, component: React.lazy(() => import("./pages/edit-classifier-type")) },

	{ path: "/classifiers/:typeCode/", layout: "private", exact: true, component: React.lazy(() => import("./pages/search-classifier")) },
	{ path: Patterns.addClassifier, layout: "private", exact: true, component: React.lazy(() => import("./pages/edit-classifier")) },
	{ path: Patterns.editClassifier, layout: "private", exact: true, component: React.lazy(() => import("./pages/edit-classifier")) }
];

AppRouteRegistry.add(Routes);
