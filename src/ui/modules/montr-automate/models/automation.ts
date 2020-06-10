import { Guid } from "@montr-core/models";

export interface IAutomation {
	uid?: Guid;
	name?: string;
	description?: string;
	condition?: IAutomationCondition;
	actions?: IAutomationAction[];
}

export interface IAutomationCondition {
	type?: string;
}

export interface IAutomationAction {
	type?: string;
}

export interface IGroupAutomationCondition extends IAutomationCondition {
	meet?: string;
	conditions?: IAutomationCondition[];
}

export interface IFieldAutomationCondition extends IAutomationCondition {
	field?: string;
	operator?: string;
	value?: string;
}

export interface ISetFieldAutomationAction extends IAutomationAction {
	field?: string;
	value?: string;
}

export interface INotifyByEmailAutomationAction extends IAutomationAction {
	recipient?: string;
	subject?: string;
	body?: string;
}
