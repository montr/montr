import { DataFieldFactory } from "@montr-core/components";
import { ComponentRegistry } from "@montr-core/services";
import React from "react";

import("./components").then(x => {
	DataFieldFactory.register("automation-condition-list", new x.AutomationConditionListFieldFactory());
	DataFieldFactory.register("automation-action-list", new x.AutomationActionListFieldFactory());

	x.AutomationConditionFactory.register("group", new x.GroupAutomationConditionFactory());
	x.AutomationConditionFactory.register("field", new x.FieldAutomationConditionFactory());
	x.AutomationActionFactory.register("set-field", new x.SetFieldAutomationActionFactory());
	x.AutomationActionFactory.register("notify-by-email", new x.NotifyByEmailAutomationActionFactory());
});

ComponentRegistry.add([
	{ path: "@montr-automate/components/pane-search-automation", component: React.lazy(() => import("./components/pane-search-automation")) }
]);
