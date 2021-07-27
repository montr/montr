import { Constants } from "@montr-core/.";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services";
import React from "react";
import { generatePath } from "react-router";

export const Api = {
	documentMetadata: `${Constants.apiURL}/document/metadata`,
	documentList: `${Constants.apiURL}/document/list`,
	documentGet: `${Constants.apiURL}/document/get`,
	documentUpdateForm: `${Constants.apiURL}/document/updateForm`,
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
	documentList: "document_list",
	documentTabs: "document_tabs",
	documentForm: "document_form"
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
