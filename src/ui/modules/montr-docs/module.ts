import React from "react";
import { generatePath } from "react-router";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services";
import { Constants } from "@montr-core/.";

export const Api = {
	documentList: `${Constants.apiURL}/document/list`,
	documentGet: `${Constants.apiURL}/document/get`,
	processList: `${Constants.apiURL}/process/list`,
};

export const Patterns = {
	editDocumentType: "/document-types/edit/:uid/:tabKey?",

	searchDocuments: "/documents/",
	viewDocument: "/documents/view/:uid/:tabKey?",
	searchProcess: "/processes/",
	editProcess: "/processes/edit/:uid/:tabKey?",
};

export const Views = {
	documentTypeTabs: "DocumentType/Tabs",

	documentList: "Document/List",

	processList: "Process/List",
	processTabs: "Process/Tabs",
};

export const RouteBuilder = {
	editDocumentType: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editDocumentType, { uid: uid.toString(), tabKey });
	},
	viewDocument: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.viewDocument, { uid: uid.toString(), tabKey });
	},
	editProcess: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editProcess, { uid: uid.toString(), tabKey });
	}
};

AppRouteRegistry.add([
	{ path: Patterns.editDocumentType, exact: true, component: React.lazy(() => import("./components/page-edit-document-type")) },
	{ path: Patterns.searchDocuments, exact: true, component: React.lazy(() => import("./components/page-search-documents")) },
	{ path: Patterns.viewDocument, exact: true, component: React.lazy(() => import("./components/page-view-document")) },
	{ path: Patterns.searchProcess, exact: true, component: React.lazy(() => import("./components/page-search-processes")) },
	{ path: Patterns.editProcess, exact: true, component: React.lazy(() => import("./components/page-edit-process")) },
]);
