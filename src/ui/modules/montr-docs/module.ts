import React from "react";
import { generatePath } from "react-router";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services";
import { Constants } from "@montr-core/.";

export const Api = {
	documentList: `${Constants.apiURL}/document/list`,
	documentGet: `${Constants.apiURL}/document/get`,
};

export const Patterns = {
	searchDocuments: "/documents/",
	viewDocument: "/documents/view/:uid/:tabKey?",
	editDocument: "/documents/edit/:uid/:tabKey?"
};

export const ClassifierTypeCode = {
	documentType: "document_type"
};

export const Views = {
	documentList: "Document/List"
};

export const RouteBuilder = {
	viewDocument: (uid: Guid | string, tabKey?: string): string => {
		return generatePath(Patterns.viewDocument, { uid: uid.toString(), tabKey });
	},
	editDocument: (uid: Guid | string, tabKey?: string): string => {
		return generatePath(Patterns.editDocument, { uid: uid.toString(), tabKey });
	},
};

AppRouteRegistry.add([
	{ path: Patterns.searchDocuments, exact: true, component: React.lazy(() => import("./components/page-search-documents")) },
	{ path: Patterns.viewDocument, exact: true, component: React.lazy(() => import("./components/page-view-document")) },
	{ path: Patterns.editDocument, exact: true, component: React.lazy(() => import("./components/page-view-document")) },
]);

ComponentRegistry.add([
	{ path: "panes/PaneProcessStepList", component: React.lazy(() => import("./components/pane-prosess-step-list")) }
]);
