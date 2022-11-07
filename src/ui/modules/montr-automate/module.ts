import { ComponentRegistry } from "@montr-core/services";
import React from "react";

import("@montr-core/components/data-field-factory").then(core => {
	import("./components/automation-field-factory").then(x => {
		core.DataFieldFactory.register("automation-condition-list", new x.AutomationConditionListFieldFactory());
		core.DataFieldFactory.register("automation-action-list", new x.AutomationActionListFieldFactory());

		x.AutomationConditionFactory.register("group", new x.GroupAutomationConditionFactory());
		// ff.AutomationConditionFactory.register("field", new x.FieldAutomationConditionFactory());
		x.AutomationActionFactory.register("set-field", new x.SetFieldAutomationActionFactory());
	});
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
