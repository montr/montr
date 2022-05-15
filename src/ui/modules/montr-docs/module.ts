import { Guid } from "@montr-core/models";
import { AppRouteRegistry, ComponentNameConvention, ComponentRegistry } from "@montr-core/services";
import React from "react";
import { generatePath } from "react-router";

export const Api = {
	documentMetadata: "/document/metadata",
	documentList: "/document/list",
	documentGet: "/document/get",
	documentUpdate: "/document/update",
	documentPublish: "/document/publish",

	documentFormUpdate: "/documentForm/update",
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
	documentList: "document-list",
	documentPage: "document-page",
	documentInfo: "document-info",
	documentForm: "document-form"
};

export const RouteBuilder = {
	viewDocument: (uid: Guid | string, tabKey?: string): string => {
		return generatePath(Patterns.viewDocument, { uid: uid.toString(), tabKey });
	}
};

AppRouteRegistry.add([
	{ path: Patterns.searchDocuments, component: React.lazy(() => import("./components/page-search-documents")) },
	{ path: Patterns.viewDocument, component: React.lazy(() => import("./components/page-view-document")) },
]);

ComponentRegistry.add([
	{ path: "@montr-docs/components/pane-view-document-info", component: React.lazy(() => import("./components/pane-view-document-info")) },
	{ path: "@montr-docs/components/pane-view-document-form", component: React.lazy(() => import("./components/pane-view-document-form")) },
	{ path: "@montr-docs/components/pane-list-prosess-step", component: React.lazy(() => import("./components/pane-list-process-step")) },

	{ path: ComponentNameConvention.entityPane("document"), component: React.lazy(() => import("./components/pane-view-document")) }
]);

