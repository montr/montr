import React from "react";
import { Layout } from "./constants";
import "./i18n";
import "./index.less";
import { AppRouteRegistry, ComponentRegistry } from "./services";

export const Api = {
	contentMenu: "/content/menu",

	metadataList: "/metadata/list",
	metadataView: "/metadata/view",
	metadataFieldTypes: "/metadata/fieldTypes",
	metadataGet: "/metadata/get",
	metadataInsert: "/metadata/insert",
	metadataUpdate: "/metadata/update",
	metadataDelete: "/metadata/delete",

	setupSave: "/setup/save",

	entityRelationList: "/entityRelation/list",

	entityStatusList: "/entityStatus/list",
	entityStatusGet: "/entityStatus/get",
	entityStatusInsert: "/entityStatus/insert",
	entityStatusUpdate: "/entityStatus/update",
	entityStatusDelete: "/entityStatus/delete",

	localeList: "/locale/list",
	localeExport: "/locale/export",
	localeImport: "/locale/import",
};

export const Patterns = {
	home: "/",
	setup: "/setup/",
	dashboard: "/dashboard/",
	locales: "/locales/",
	settings: "/settings/",

	profile: "/profile/",
	accountRegister: "/account/register/",
};

export const Views = {
	fieldMetadataList: "field-metadata-list",
	fieldmetadataForm: "field-metadata-form",

	setupForm: "Setup/Form",

	entityStatusList: "EntityStatus/Grid",
	entityStatusForm: "EntityStatus/Form",
};

import("./components").then(x => {
	x.DataFieldFactory.register("boolean", new x.BooleanFieldFactory());
	x.DataFieldFactory.register("number", new x.NumberFieldFactory());
	x.DataFieldFactory.register("text", new x.TextFieldFactory());
	x.DataFieldFactory.register("textarea", new x.TextAreaFieldFactory());
	x.DataFieldFactory.register("select", new x.SelectFieldFactory());
	x.DataFieldFactory.register("select-options", new x.DesignSelectOptionsFieldFactory());
	x.DataFieldFactory.register("password", new x.PasswordFieldFactory());
	x.DataFieldFactory.register("date", new x.DateFieldFactory());
});

AppRouteRegistry.add([
	{ path: Patterns.home, layout: Layout.public, exact: true, component: React.lazy(() => import("./components/page-home")) },
	{ path: Patterns.setup, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-setup")) },

	{ path: Patterns.dashboard, exact: true, component: React.lazy(() => import("./components/page-dashboard")) },
	{ path: Patterns.locales, exact: true, component: React.lazy(() => import("./components/page-search-locale-string")) },
	{ path: Patterns.settings, exact: true, component: React.lazy(() => import("./components/page-settings")) },
]);

ComponentRegistry.add([
	{ path: "@montr-core/components/button-edit", component: React.lazy(() => import("./components/button-edit")) },

	{ path: "@montr-core/components/pane-edit-fields-metadata", component: React.lazy(() => import("./components/pane-edit-fields-metadata")) },
	{ path: "@montr-core/components/pane-edit-form-metadata", component: React.lazy(() => import("./components/pane-edit-form-metadata")) },
	{ path: "@montr-core/components/pane-search-entity-statuses", component: React.lazy(() => import("./components/pane-search-entity-statuses")) },
	{ path: "@montr-core/components/pane-view-related-entities", component: React.lazy(() => import("./components/pane-view-related-entities")) },
]);
