import { DataFieldFactory } from "@montr-core/components";
import { AutomationConditionFieldFactory, AutomationActionListFieldFactory } from "./components";

import("./components").then(x => {
	DataFieldFactory.register("automation-condition", new AutomationConditionFieldFactory());
	DataFieldFactory.register("automation-action-list", new AutomationActionListFieldFactory());
});
