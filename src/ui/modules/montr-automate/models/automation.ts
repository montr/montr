import { Classifier } from "@montr-master-data/models";

export interface Automation extends Classifier {
	entityTypeCode?: string;
	automationTypeCode?: string;
	conditions?: AutomationCondition[];
	actions?: AutomationAction[];
}

export interface AutomationRuleType {
	code: string;
	name: string;
	icon: string;
}

export interface AutomationCondition {
	type?: string;
}

export interface AutomationAction {
	type?: string;
}

export interface GroupAutomationCondition extends AutomationCondition {
	meet?: string;
	conditions?: AutomationCondition[];
}

export interface FieldAutomationCondition extends AutomationCondition {
	props?: {
		field?: string;
		operator?: string;
		value?: string;
	};
}

export interface SetFieldAutomationAction extends AutomationAction {
	field?: string;
	value?: string;
}

export interface NotifyByEmailAutomationAction extends AutomationAction {
	recipient?: string;
	subject?: string;
	body?: string;
}
