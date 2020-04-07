import React from "react";
import { generatePath } from "react-router";
import { Guid } from "@montr-core/models/guid";
import { AppRouteRegistry } from "@montr-core/services/app-route-registry";
import { DataFieldFactory } from "@montr-core/components/data-field-factory";
import { Constants } from "@montr-core/.";

import("./components").then(x => {
	DataFieldFactory.register("classifier-group", new x.ClassifierGroupFieldFactory());
	DataFieldFactory.register("classifier", new x.ClassifierFieldFactory());
	DataFieldFactory.register("select-classifier-type", new x.SelectClassifierTypeFieldFactory());
});

export const Api = {
	classifierList: `${Constants.apiURL}/classifier/list`,
	classifierExport: `${Constants.apiURL}/classifier/export`,
	classifierCreate: `${Constants.apiURL}/classifier/create`,
	classifierGet: `${Constants.apiURL}/classifier/get`,
	classifierInsert: `${Constants.apiURL}/classifier/insert`,
	classifierUpdate: `${Constants.apiURL}/classifier/update`,
	classifierDelete: `${Constants.apiURL}/classifier/delete`,

	numeratorList: `${Constants.apiURL}/numerator/list`,
	numeratorCreate: `${Constants.apiURL}/numerator/create`,
	numeratorGet: `${Constants.apiURL}/numerator/get`,
	numeratorInsert: `${Constants.apiURL}/numerator/insert`,
	numeratorUpdate: `${Constants.apiURL}/numerator/update`,
	numeratorDelete: `${Constants.apiURL}/numerator/delete`,
};

export const Views = {
	classifierList: "Classifier/Grid",
	numeratorList: "Numerator/Grid",

	formEditNumerator: "Numerator/Form",
};

export const Patterns = {
	searchClassifierType: "/classifiers/",
	addClassifierType: "/classifiers/add/",
	editClassifierType: "/classifiers/edit/:uid/:tabKey?",
	searchClassifier: "/classifiers/:typeCode/",
	addClassifier: "/classifiers/:typeCode/add/:parentUid?",
	editClassifier: "/classifiers/:typeCode/edit/:uid/:tabKey?",
	searchNumerator: "/numerators/",

	editNumerator: "/numerators/:uid/:tabKey?",
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
	editNumerator: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editNumerator, { uid: uid.toString(), tabKey });
	},
};

AppRouteRegistry.add([
	{ path: Patterns.searchClassifierType, exact: true, component: React.lazy(() => import("./components/page-search-classifier-type")) },
	{ path: Patterns.addClassifierType, exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: Patterns.editClassifierType, exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: Patterns.searchClassifier, exact: true, component: React.lazy(() => import("./components/page-search-classifier")) },
	{ path: Patterns.addClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) },
	{ path: Patterns.editClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) },
	{ path: Patterns.searchNumerator, exact: true, component: React.lazy(() => import("./components/page-search-numerator")) },

	{ path: Patterns.editNumerator, exact: true, component: React.lazy(() => import("./components/page-edit-numerator")) }
]);
