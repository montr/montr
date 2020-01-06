import { generatePath } from "react-router";
import { Guid } from "@montr-core/models";
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

AppRouteRegistry.add([
	{ path: "/classifiers/", exact: true, component: React.lazy(() => import("./components/page-search-classifier-type")) },
	{ path: "/classifiers/add/", exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: Patterns.editClassifierType, exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: "/classifiers/:typeCode/", exact: true, component: React.lazy(() => import("./components/page-search-classifier")) },
	{ path: Patterns.addClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) },
	{ path: Patterns.editClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) }
]);
