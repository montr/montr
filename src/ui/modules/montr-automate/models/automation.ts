import { Guid } from "@montr-core/models";

export interface Automation {
	uid?: Guid;
	name?: string;
	typeCode?: string;
	description?: string;
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
	field?: string;
	props?: {
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
