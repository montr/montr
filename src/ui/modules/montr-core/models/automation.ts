import { Guid } from ".";

export interface IAutomation {
	uid?: Guid;
	name?: string;
	description?: string;
}

export interface IAutomationCondition {
	type?: string;
}

export interface IAutomationAction {
	type?: string;
}
