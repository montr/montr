import { Constants } from "@montr-core/.";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services";
import React from "react";
import { generatePath } from "react-router";

export const Api = {
	documentMetadataView: `${Constants.apiURL}/documentMetadata/view`,

	documentList: `${Constants.apiURL}/document/list`,
	documentGet: `${Constants.apiURL}/document/get`,
};

export const Patterns = {
	searchDocuments: "/documents/",
	viewDocument: "/documents/view/:uid/:tabKey?",
};

export const EntityTypeCode = {
	document: "document"
};

export const ClassifierTypeCode = {
	documentType: "document_type"
};

export const Views = {
	documentList: "Document/List",

	documentTabs: "document_tabs"
};

export const RouteBuilder = {
	viewDocument: (uid: Guid | string, tabKey?: string): string => {
		return generatePath(Patterns.viewDocument, { uid: uid.toString(), tabKey });
	}
};

AppRouteRegistry.add([
	{ path: Patterns.searchDocuments, exact: true, component: React.lazy(() => import("./components/page-search-documents")) },
	{ path: Patterns.viewDocument, exact: true, component: React.lazy(() => import("./components/page-view-document")) },
]);

ComponentRegistry.add([
	{ path: "pane_view_document_form", component: React.lazy(() => import("./components/pane-view-document-form")) },
	{ path: "panes/PaneProcessStepList", component: React.lazy(() => import("./components/pane-prosess-step-list")) }
]);
