import React from "react";
import { Constants, Layout } from "./constants";
import "./i18n";
import "./index.less";
import { AppRouteRegistry, ComponentRegistry } from "./services";


export const Api = {
	contentMenu: `${Constants.apiURL}/content/menu`,

	metadataList: `${Constants.apiURL}/metadata/list`,
	metadataView: `${Constants.apiURL}/metadata/view`,
	metadataFieldTypes: `${Constants.apiURL}/metadata/fieldTypes`,
	metadataGet: `${Constants.apiURL}/metadata/get`,
	metadataInsert: `${Constants.apiURL}/metadata/insert`,
	metadataUpdate: `${Constants.apiURL}/metadata/update`,
	metadataDelete: `${Constants.apiURL}/metadata/delete`,

	setupSave: `${Constants.apiURL}/setup/save`,

	entityStatusList: `${Constants.apiURL}/entityStatus/list`,
	entityStatusGet: `${Constants.apiURL}/entityStatus/get`,
	entityStatusInsert: `${Constants.apiURL}/entityStatus/insert`,
	entityStatusUpdate: `${Constants.apiURL}/entityStatus/update`,
	entityStatusDelete: `${Constants.apiURL}/entityStatus/delete`,
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
	formMetadataList: "form-metadata-list",
	formMetadataForm: "form-metadata-form",

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
	{ path: "@montr-core/components/pane-edit-fields-metadata", component: React.lazy(() => import("./components/pane-edit-fields-metadata")) },
	{ path: "@montr-core/components/pane-edit-form-metadata", component: React.lazy(() => import("./components/pane-edit-form-metadata")) },
	{ path: "@montr-core/components/pane-search-entity-statuses", component: React.lazy(() => import("./components/pane-search-entity-statuses")) }
]);
