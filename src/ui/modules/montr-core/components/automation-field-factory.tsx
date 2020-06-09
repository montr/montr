import React from "react";
import {
	IAutomationCondition, IAutomationAction, IIndexer,
	IFieldAutomationCondition, IGroupAutomationCondition,
	ISetFieldAutomationAction, INotifyByEmailAutomationAction
} from "../models";
import { FieldData } from "../models/field-data";
import { NotifyByEmailAutomationAction } from "./notify-by-email-automation-action";
import { SetFieldAutomationAction } from "./set-field-automation-action";

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

export class IAutomationActionProps {
	// data: IIndexer;
	item: FieldData;
	typeSelector: React.ReactElement;
}

export abstract class AutomationActionFactory<TAction extends IAutomationAction> {
	private static Map: { [key: string]: AutomationActionFactory<IAutomationAction>; } = {};

	static register(key: string, factory: AutomationActionFactory<IAutomationAction>) {
		AutomationActionFactory.Map[key] = factory;
	}

	static get(key: string): AutomationActionFactory<IAutomationAction> {
		return AutomationActionFactory.Map[key];
	}

	abstract createFormItem(action: TAction, props: IAutomationActionProps): React.ReactNode;
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
	createFormItem(action: ISetFieldAutomationAction, props: IAutomationActionProps): React.ReactElement {
		return <SetFieldAutomationAction action={action} {...props} />;
	}
}

class NotifyByEmailAutomationActionFactory extends AutomationConditionFactory<INotifyByEmailAutomationAction> {
	createFormItem(action: INotifyByEmailAutomationAction, props: IAutomationActionProps): React.ReactElement {
		return <NotifyByEmailAutomationAction action={action} {...props} />;
	}
}

AutomationConditionFactory.register("group", new GroupAutomationConditionFactory());
AutomationConditionFactory.register("field", new FieldAutomationConditionFactory());
AutomationActionFactory.register("set-field", new SetFieldAutomationActionFactory());
AutomationActionFactory.register("notify-by-email", new NotifyByEmailAutomationActionFactory());
