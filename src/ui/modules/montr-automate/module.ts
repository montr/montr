import { DataFieldFactory } from "@montr-core/components";
import { AutomationConditionListFieldFactory, AutomationActionListFieldFactory } from "./components";

import("./components").then(x => {
	DataFieldFactory.register("automation-condition-list", new AutomationConditionListFieldFactory());
	DataFieldFactory.register("automation-action-list", new AutomationActionListFieldFactory());
});
