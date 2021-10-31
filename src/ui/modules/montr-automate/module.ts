import { DataFieldFactory } from "@montr-core/components";
import { ComponentRegistry } from "@montr-core/services";
import React from "react";
import { AutomationActionFactory, AutomationConditionFactory } from "./components";

import("./components").then(x => {
	DataFieldFactory.register("automation-condition-list", new x.AutomationConditionListFieldFactory());
	DataFieldFactory.register("automation-action-list", new x.AutomationActionListFieldFactory());

	AutomationConditionFactory.register("group", new x.GroupAutomationConditionFactory());
	AutomationConditionFactory.register("field", new x.FieldAutomationConditionFactory());

	AutomationActionFactory.register("set-field", new x.SetFieldAutomationActionFactory());
});

export const Api = {
	automationList: "/automation/list/",
	automationMetadata: "/automation/metadata/",
	automationGet: "/automation/get/",
	automationActionTypes: "/automation/actionTypes/",
	automationConditionTypes: "/automation/conditionTypes/",
	automationInsert: "/automation/insert/",
	automationUpdate: "/automation/update/",
	automationDelete: "/automation/delete/",

	fieldAutomationConditionFields: "/fieldAutomationCondition/fields/"
};

export const Views = {
	automationList: "automation-list",
	automationForm: "automation-form"
};

ComponentRegistry.add([
	{ path: "@montr-automate/components/pane-search-automation", component: React.lazy(() => import("./components/pane-search-automation")) }
]);
