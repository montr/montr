import React from "react";
import { AppRouteRegistry, ComponentRegistry } from "./services";
import { Constants, Layout } from "./constants";

import "./i18n";
import "./index.less";

export const Api = {
	metadataList: `${Constants.apiURL}/metadata/list`,

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
	metadataList: "Metadata/Grid",

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
	{ path: "panes/PaneSearchMetadata", component: React.lazy(() => import("./components/pane-search-metadata")) },
	{ path: "panes/PaneSearchEntityStatuses", component: React.lazy(() => import("./components/pane-search-entity-statuses")) }
]);
