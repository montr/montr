import { DataFieldFactory } from "@montr-core/components/data-field-factory";
import { ComponentRegistry } from "@montr-core/services";
import React from "react";
import { AutomationActionFactory, AutomationConditionFactory } from "./components";

import("./components").then(x => {
	DataFieldFactory.register("automation-condition-list", new x.AutomationConditionListFieldFactory());
	DataFieldFactory.register("automation-action-list", new x.AutomationActionListFieldFactory());

	AutomationConditionFactory.register("group", new x.GroupAutomationConditionFactory());
	// AutomationConditionFactory.register("field", new x.FieldAutomationConditionFactory());

	AutomationActionFactory.register("set-field", new x.SetFieldAutomationActionFactory());
});

export const Api = {
	automationActionMetadata: "/automation/actionMetadata/",
	automationConditionMetadata: "/automation/conditionMetadata/",
	automationActionTypes: "/automation/actionTypes/",
	automationConditionTypes: "/automation/conditionTypes/",

	automationUpdateRules: "/automation/updateRules/",

	fieldAutomationConditionFields: "/fieldAutomationCondition/fields/"
};

export const Views = {
	automationList: "automation-list",
	automationForm: "automation-form"
};

ComponentRegistry.add([
	{ path: "@montr-automate/components/pane-edit-automation", component: React.lazy(() => import("./components/pane-edit-automation")) },
]);
