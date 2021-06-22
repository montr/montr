import React from "react";
import { generatePath } from "react-router";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/app-route-registry";
import { DataFieldFactory } from "@montr-core/components";
import { Constants } from "@montr-core/.";
import { ComponentRegistry } from "@montr-core/services";

import("./components").then(x => {
	DataFieldFactory.register("classifier-group", new x.ClassifierGroupFieldFactory());
	DataFieldFactory.register("classifier", new x.ClassifierFieldFactory());
	DataFieldFactory.register("select-classifier-type", new x.SelectClassifierTypeFieldFactory());
});

export const EntityTypeCode = {
	classifier: "classifier"
};

export const Locale = {
	Namespace: "master-data"
};

export const Api = {
	classifierMetadataView: `${Constants.apiURL}/classifierMetadata/view`,

	classifierList: `${Constants.apiURL}/classifier/list`,
	classifierExport: `${Constants.apiURL}/classifier/export`,
	classifierCreate: `${Constants.apiURL}/classifier/create`,
	classifierGet: `${Constants.apiURL}/classifier/get`,
	classifierInsert: `${Constants.apiURL}/classifier/insert`,
	classifierUpdate: `${Constants.apiURL}/classifier/update`,
	classifierDelete: `${Constants.apiURL}/classifier/delete`,

	numeratorEntityList: `${Constants.apiURL}/numeratorEntity/list`,
	numeratorEntityGet: `${Constants.apiURL}/numeratorEntity/get`,
	numeratorEntitySave: `${Constants.apiURL}/numeratorEntity/save`,
};

export const Views = {
	classifierTypeTabs: "ClassifierType/Tabs",
	classifierTypeForm: "ClassifierType",

	classifierTabs: "Classifier/Tabs",
	classifierForm: "Classifier/Form",
	classifierList: "Classifier/Grid",

	numeratorEntityForm: "NumeratorEntity/Form",
	numeratorEntityList: "NumeratorEntity/Grid",
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
	editClassifierType: (uid: Guid | string, tabKey?: string): string => {
		return generatePath(Patterns.editClassifierType, { uid: uid.toString(), tabKey });
	},

	addClassifier: (typeCode: string, parentUid: Guid | string): string => {
		return generatePath(Patterns.addClassifier, { typeCode, parentUid: parentUid ? parentUid.toString() : null });
	},
	editClassifier: (typeCode: string, uid: Guid | string, tabKey?: string): string => {
		return generatePath(Patterns.editClassifier, { typeCode, uid: uid.toString(), tabKey });
	},
};

AppRouteRegistry.add([
	{ path: Patterns.searchClassifierType, exact: true, component: React.lazy(() => import("./components/page-search-classifier-type")) },
	{ path: Patterns.addClassifierType, exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: Patterns.editClassifierType, exact: true, component: React.lazy(() => import("./components/page-edit-classifier-type")) },

	{ path: Patterns.searchClassifier, exact: true, component: React.lazy(() => import("./components/page-search-classifier")) },
	{ path: Patterns.addClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) },
	{ path: Patterns.editClassifier, exact: true, component: React.lazy(() => import("./components/page-edit-classifier")) },
]);

ComponentRegistry.add([
	{ path: "panes/TabEditClassifierType", component: React.lazy(() => import("./components/tab-edit-classifier-type")) },
	{ path: "panes/TabEditClassifierTypeHierarchy", component: React.lazy(() => import("./components/tab-edit-classifier-type-hierarchy")) },

	{ path: "panes/TabEditClassifier", component: React.lazy(() => import("./components/tab-edit-classifier")) },
	{ path: "panes/TabEditClassifierHierarchy", component: React.lazy(() => import("./components/tab-edit-classifier-hierarchy")) },

	{ path: "panes/TabEditNumeratorEntities", component: React.lazy(() => import("./components/tab-edit-numerator-entities")) },
	{ path: "panes/PaneEditNumeration", component: React.lazy(() => import("./components/tab-edit-numeration")) },
]);
