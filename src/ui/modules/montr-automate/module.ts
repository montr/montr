import { Constants } from "@montr-core/.";
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

export const Api = {
	automationList: `${Constants.apiURL}/automation/list/`,
	automationGet: `${Constants.apiURL}/automation/get/`,
	automationActionTypes: `${Constants.apiURL}/automation/actionTypes/`,
	automationConditionTypes: `${Constants.apiURL}/automation/conditionTypes/`,
	automationInsert: `${Constants.apiURL}/automation/insert/`,
	automationUpdate: `${Constants.apiURL}/automation/update/`,
	automationDelete: `${Constants.apiURL}/automation/delete/`,

	fieldAutomationConditionFields: `${Constants.apiURL}/fieldAutomationCondition/fields/`
};

export const Views = {
	automationList: "Automation/Grid",
	automationEdit: "Automation/Edit"
};

ComponentRegistry.add([
	{ path: "@montr-automate/components/pane-search-automation", component: React.lazy(() => import("./components/pane-search-automation")) }
]);
