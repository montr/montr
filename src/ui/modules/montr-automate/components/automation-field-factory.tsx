import React from "react";
import { IIndexer } from "@montr-core/models";
import { FieldData } from "@montr-core/models/field-data";
import { DataFieldFactory, IDataFormOptions } from "@montr-core/components";
import { IAutomationConditionListField, IAutomationActionListField, IAutomationCondition, IAutomationAction, IGroupAutomationCondition, IFieldAutomationCondition, ISetFieldAutomationAction, INotifyByEmailAutomationAction } from "../models/";
import { AutomationConditionList, AutomationActionList, GroupAutomationCondition, FieldAutomationCondition, SetFieldAutomationAction, NotifyByEmailAutomationAction } from ".";

export class AutomationConditionListFieldFactory extends DataFieldFactory<IAutomationConditionListField> {

	createFormItem = (field: IAutomationConditionListField, data: IIndexer, options: IDataFormOptions): React.ReactNode => {
		return <AutomationConditionList field={field} />;
	};

	createEditNode(field: IAutomationConditionListField, data: IIndexer): React.ReactElement {
		return null;
	}

	createViewNode(field: IAutomationConditionListField, data: IIndexer): React.ReactElement {
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

export class IAutomationConditionProps {
	item: FieldData;
	typeSelector: React.ReactElement;
}

export abstract class AutomationConditionFactory<TCondition extends IAutomationCondition> {
	private static Map: { [key: string]: AutomationConditionFactory<IAutomationCondition>; } = {};

	static register(key: string, factory: AutomationConditionFactory<IAutomationCondition>) {
		AutomationConditionFactory.Map[key] = factory;
	}

	static get(key: string): AutomationConditionFactory<IAutomationCondition> {
		return AutomationConditionFactory.Map[key];
	}

	abstract createFormItem(condition: TCondition, props: IAutomationConditionProps): React.ReactNode;
}

export class IAutomationActionProps {
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
	createFormItem(condition: IGroupAutomationCondition, props: IAutomationConditionProps): React.ReactElement {
		return <GroupAutomationCondition condition={condition} {...props} />;
	}
}

class FieldAutomationConditionFactory extends AutomationConditionFactory<IFieldAutomationCondition> {
	createFormItem(condition: IFieldAutomationCondition, props: IAutomationConditionProps): React.ReactElement {
		return <FieldAutomationCondition condition={condition} {...props} />;
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
