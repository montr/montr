import {
	IAutomationCondition, IAutomationAction, IIndexer,
	IFieldAutomationCondition, IGroupAutomationCondition,
	ISetFieldAutomationAction, INotifyByEmailAutomationAction
} from "../models";
import React from "react";

export abstract class AutomationConditionFactory<TCondition extends IAutomationCondition> {
	private static Map: { [key: string]: AutomationConditionFactory<IAutomationCondition>; } = {};

	static register(key: string, factory: AutomationConditionFactory<IAutomationCondition>) {
		AutomationConditionFactory.Map[key] = factory;
	}

	static get(key: string): AutomationConditionFactory<IAutomationCondition> {
		return AutomationConditionFactory.Map[key];
	}

	abstract createFormItem(condition: TCondition, data: IIndexer): React.ReactNode;
}

export abstract class AutomationActionFactory<TAction extends IAutomationAction> {
	private static Map: { [key: string]: AutomationActionFactory<IAutomationAction>; } = {};

	static register(key: string, factory: AutomationActionFactory<IAutomationAction>) {
		AutomationActionFactory.Map[key] = factory;
	}

	static get(key: string): AutomationActionFactory<IAutomationAction> {
		return AutomationActionFactory.Map[key];
	}

	abstract createFormItem(action: TAction, data: IIndexer): React.ReactNode;
}

class GroupAutomationConditionFactory extends AutomationConditionFactory<IGroupAutomationCondition> {
	createFormItem(condition: IGroupAutomationCondition, data: IIndexer): React.ReactElement {
		return <h1>IGroupAutomationCondition</h1>;
	}
}

class FieldAutomationConditionFactory extends AutomationConditionFactory<IFieldAutomationCondition> {
	createFormItem(condition: IFieldAutomationCondition, data: IIndexer): React.ReactElement {
		return <h1>IFieldAutomationCondition</h1>;
	}
}

class SetFieldAutomationActionFactory extends AutomationConditionFactory<ISetFieldAutomationAction> {
	createFormItem(action: ISetFieldAutomationAction, data: IIndexer): React.ReactElement {
		return <h1>ISetFieldAutomationAction</h1>;
	}
}

class NotifyByEmailAutomationActionFactory extends AutomationConditionFactory<INotifyByEmailAutomationAction> {
	createFormItem(action: INotifyByEmailAutomationAction, data: IIndexer): React.ReactElement {
		return <h1>INotifyByEmailAutomationAction</h1>;
	}
}

AutomationConditionFactory.register("group", new GroupAutomationConditionFactory());
AutomationConditionFactory.register("field", new FieldAutomationConditionFactory());
AutomationActionFactory.register("set-field", new SetFieldAutomationActionFactory());
AutomationActionFactory.register("notify-by-email", new NotifyByEmailAutomationActionFactory());
