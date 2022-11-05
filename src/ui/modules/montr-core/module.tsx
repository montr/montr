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

import("./components/data-field-factory").then(x => {
	x.DataFieldFactory.register("boolean", new x.BooleanFieldFactory());
	x.DataFieldFactory.register("number", new x.NumberFieldFactory());
	x.DataFieldFactory.register("text", new x.TextFieldFactory());
	x.DataFieldFactory.register("textarea", new x.TextAreaFieldFactory());
	x.DataFieldFactory.register("select", new x.SelectFieldFactory());
	x.DataFieldFactory.register("select-options", new x.DesignSelectOptionsFieldFactory());
	x.DataFieldFactory.register("password", new x.PasswordFieldFactory());
	x.DataFieldFactory.register("date", new x.DateFieldFactory());
});

const PageSetup = React.lazy(() => import("./components/page-setup"));
const PageHome = React.lazy(() => import("./components/page-home"));
const PageDashboard = React.lazy(() => import("./components/page-dashboard"));
const PageSearchLocaleString = React.lazy(() => import("./components/page-search-locale-string"));
const PageSettings = React.lazy(() => import("./components/page-settings"));

AppRouteRegistry.add(Layout.auth, [
	{ path: Patterns.setup, element: <PageSetup /> }
]);

AppRouteRegistry.add(Layout.public, [
	{ path: Patterns.home, element: <PageHome /> },
]);

AppRouteRegistry.add(Layout.private, [
	{ path: Patterns.dashboard, element: <PageDashboard /> },
	{ path: Patterns.locales, element: <PageSearchLocaleString /> },
	{ path: Patterns.settings, element: <PageSettings /> },
]);

ComponentRegistry.add([
	{ path: "@montr-core/components/button-edit", component: React.lazy(() => import("./components/button-edit")) },

	{ path: "@montr-core/components/pane-edit-fields-metadata", component: React.lazy(() => import("./components/pane-edit-fields-metadata")) },
	{ path: "@montr-core/components/pane-edit-form-metadata", component: React.lazy(() => import("./components/pane-edit-form-metadata")) },
	{ path: "@montr-core/components/pane-search-entity-statuses", component: React.lazy(() => import("./components/pane-search-entity-statuses")) },
	{ path: "@montr-core/components/pane-view-related-entities", component: React.lazy(() => import("./components/pane-view-related-entities")) },
]);
