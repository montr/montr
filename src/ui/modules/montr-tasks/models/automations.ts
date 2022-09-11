import { AutomationAction } from "@montr-automate/models";

export interface CreateTaskAutomationAction extends AutomationAction {
	recipient?: string;
	subject?: string;
	body?: string;
}
