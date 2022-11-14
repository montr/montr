import { Layout } from "@montr-core/constants";
import { AppRouteRegistry } from "@montr-core/services/app-route-registry";
import { ComponentRegistry } from "@montr-core/services/component-registry";
import React from "react";

export const Locale = {
	Namespace: "settings"
};

export const Api = {
	settingsMetadata: "/settings/metadata",
	settingsGet: "/settings/get",
	settingsUpdate: "/settings/update"
};

export const Patterns = {
	settings: "/settings/"
};

const PageSettings = React.lazy(() => import("./components/page-settings"));

AppRouteRegistry.add(Layout.private, [
	{ path: Patterns.settings, element: <PageSettings /> },
]);

ComponentRegistry.add([
	{ path: "@montr-settings/components/pane-settings", component: React.lazy(() => import("./components/pane-settings")) },
]);
