import React from "react";
import { generatePath } from "react-router";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/";
import { DataFieldFactory } from "@montr-core/components";
import { ClassifierGroupFieldFactory, ClassifierFieldFactory, SelectClassifierTypeFieldFactory } from "./components";
import { Constants } from "@montr-core/.";

export const Api = {
	classifierList: `${Constants.apiURL}/classifier/list`,
};

export const Views = {
	classifierList: "Classifier/Grid",
};

export const Patterns = {
	searchClassifierType: "/classifiers/",
	addClassifierType: "/classifiers/add/",
	editClassifierType: "/classifiers/edit/:uid/:tabKey?",
	searchClassifier: "/classifiers/:typeCode/",
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
	{ path: Patterns.searchClassifierType, exact: true, component: React.lazy(() => import("./components/page-search-classifier-type")) },
	{ path: Patterns.addClassifierType, exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: Patterns.editClassifierType, exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: Patterns.searchClassifier, exact: true, component: React.lazy(() => import("./components/page-search-classifier")) },
	{ path: Patterns.addClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) },
	{ path: Patterns.editClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) }
]);

DataFieldFactory.register("classifier-group", new ClassifierGroupFieldFactory());
DataFieldFactory.register("classifier", new ClassifierFieldFactory());
DataFieldFactory.register("select-classifier-type", new SelectClassifierTypeFieldFactory());
