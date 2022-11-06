import { ComponentRegistry } from "@montr-core/services/component-registry";
import React from "react";

ComponentRegistry.add([
	{ path: "@montr-settings/components/pane-settings", component: React.lazy(() => import("./components/pane-settings")) },
]);
