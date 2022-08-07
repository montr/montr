import { DataFieldFactory } from "@montr-core/components";
import { Layout } from "@montr-core/constants";
import { Guid } from "@montr-core/models";
import { ComponentRegistry } from "@montr-core/services";
import { AppRouteRegistry } from "@montr-core/services/app-route-registry";
import React from "react";
import { generatePath } from "react-router";

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
	classifierMetadataView: "/classifierMetadata/view",

	classifierTypeMetadata: "/classifierType/metadata",
	classifierTypeList: "/classifierType/list",
	classifierTypeGet: "/classifierType/get",
	classifierTypeCreate: "/classifierType/create",
	classifierTypeInsert: "/classifierType/insert",
	classifierTypeUpdate: "/classifierType/update",
	classifierTypeDelete: "/classifierType/delete",

	classifierList: "/classifier/list",
	classifierExport: "/classifier/export",
	classifierCreate: "/classifier/create",
	classifierGet: "/classifier/get",
	classifierInsert: "/classifier/insert",
	classifierUpdate: "/classifier/update",
	classifierDelete: "/classifier/delete",

	classifierGroupList: "/classifierGroup/list",
	classifierGroupGet: "/classifierGroup/get",
	classifierGroupInsert: "/classifierGroup/insert",
	classifierGroupUpdate: "/classifierGroup/update",
	classifierGroupDelete: "/classifierGroup/delete",

	classifierLinkList: "/classifierLink/list",
	classifierLinkInsert: "/classifierLink/insert",
	classifierLinkDelete: "/classifierLink/delete",

	classifierTreeList: "/classifierTree/list",
	classifierTreeGet: "/classifierTree/get",
	classifierTreeInsert: "/classifierTree/insert",
	classifierTreeUpdate: "/classifierTree/update",
	classifierTreeDelete: "/classifierTree/delete",

	numeratorEntityList: "/numeratorEntity/list",
	numeratorEntityGet: "/numeratorEntity/get",
	numeratorEntitySave: "/numeratorEntity/save",
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
	editClassifierType: (uid: Guid | string, tabKey = "info"): string => {
		return generatePath(Patterns.editClassifierType, { uid: uid.toString(), tabKey });
	},

	searchClassifier: (typeCode: string): string => {
		return generatePath(Patterns.searchClassifier, { typeCode });
	},
	addClassifier: (typeCode: string, parentUid: Guid | string): string => {
		return generatePath(Patterns.addClassifier, { typeCode, parentUid: parentUid ? parentUid.toString() : null });
	},
	editClassifier: (typeCode: string, uid: Guid | string, tabKey = "info"): string => {
		return generatePath(Patterns.editClassifier, { typeCode, uid: uid.toString(), tabKey });
	},
};

AppRouteRegistry.add([
	{ path: Patterns.searchClassifierType, layout: Layout.private, component: React.lazy(() => import("./components/page-search-classifier-type")) },
	{ path: Patterns.addClassifierType, layout: Layout.private, component: React.lazy(() => import("./components/page-edit-classifier-type")) },
	{ path: Patterns.editClassifierType, layout: Layout.private, component: React.lazy(() => import("./components/page-edit-classifier-type")) },

	{ path: Patterns.searchClassifier, layout: Layout.private, component: React.lazy(() => import("./components/page-search-classifier")) },
	{ path: Patterns.addClassifier, layout: Layout.private, component: React.lazy(() => import("./components/page-edit-classifier")) },
	{ path: Patterns.editClassifier, layout: Layout.private, component: React.lazy(() => import("./components/page-edit-classifier")) },
]);

ComponentRegistry.add([
	{ path: "@montr-master-data/components/tab-edit-classifier-type", component: React.lazy(() => import("./components/tab-edit-classifier-type")) },
	{ path: "@montr-master-data/components/tab-edit-classifier-type-hierarchy", component: React.lazy(() => import("./components/tab-edit-classifier-type-hierarchy")) },

	{ path: "@montr-master-data/components/tab-edit-classifier", component: React.lazy(() => import("./components/tab-edit-classifier")) },
	{ path: "@montr-master-data/components/tab-edit-classifier-hierarchy", component: React.lazy(() => import("./components/tab-edit-classifier-hierarchy")) },

	{ path: "@montr-master-data/components/tab-edit-numeration", component: React.lazy(() => import("./components/tab-edit-numeration")) },
	{ path: "@montr-master-data/components/tab-edit-numerator-entities", component: React.lazy(() => import("./components/tab-edit-numerator-entities")) },
]);
