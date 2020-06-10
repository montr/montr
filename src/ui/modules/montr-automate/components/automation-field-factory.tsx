import React from "react";
import { IIndexer } from "@montr-core/models";
import { FieldData } from "@montr-core/models/field-data";
import { NotifyByEmailAutomationAction } from "./notify-by-email-automation-action";
import { SetFieldAutomationAction } from "./set-field-automation-action";
import {
	IAutomationCondition, IAutomationAction, IGroupAutomationCondition,
	INotifyByEmailAutomationAction, ISetFieldAutomationAction, IFieldAutomationCondition
} from "../models/automation";
import { DataFieldFactory, IDataFormOptions } from "@montr-core/components";
import { IAutomationConditionField, IAutomationActionListField } from "../models/automation-field";
import { AutomationCondition } from "./automation-condition";
import { AutomationActionList } from "./automation-action-list";

export class AutomationConditionFieldFactory extends DataFieldFactory<IAutomationConditionField> {

	createFormItem = (field: IAutomationConditionField, data: IIndexer, options: IDataFormOptions): React.ReactNode => {
		return <AutomationCondition field={field} />;
	};

	createEditNode(field: IAutomationConditionField, data: IIndexer): React.ReactElement {
		return null;
	}

	createViewNode(field: IAutomationConditionField, data: IIndexer): React.ReactElement {
		return null;
	}
}

export class AutomationActionListFieldFactory extends DataFieldFactory<IAutomationActionListField> {

	createFormItem = (field: IAutomationActionListField, data: IIndexer, options: IDataFormOptions): React.ReactNode => {
		return <AutomationActionList field={field} />;
	};

	createEditNode(field: IAutomationActionListField, data: IIndexer): React.ReactElement {
		return null;
	}

	createViewNode(field: IAutomationActionListField, data: IIndexer): React.ReactElement {
		return null;
	}
}

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
