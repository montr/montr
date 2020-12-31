import React from "react";
import { IIndexer, FieldData } from "@montr-core/models";
import { DataFieldFactory, DataFormOptions } from "@montr-core/components";
import { AutomationConditionListField, AutomationActionListField, AutomationCondition, AutomationAction, GroupAutomationCondition, FieldAutomationCondition, SetFieldAutomationAction, NotifyByEmailAutomationAction } from "../models/";
import { AutomationConditionList, AutomationActionList, GroupAutomationConditionItem, FieldAutomationConditionItem, SetFieldAutomationActionItem, NotifyByEmailAutomationActionItem } from ".";

export class AutomationConditionListFieldFactory extends DataFieldFactory<AutomationConditionListField> {

	createFormItem = (field: AutomationConditionListField, data: IIndexer, options: DataFormOptions): React.ReactNode => {
		return <AutomationConditionList key={field.key} field={field} options={options} />;
	};

	createEditNode(field: AutomationConditionListField, data: IIndexer): React.ReactElement {
		return null;
	}

	createViewNode(field: AutomationConditionListField, data: IIndexer): React.ReactElement {
		return null;
	}
}

export class AutomationActionListFieldFactory extends DataFieldFactory<AutomationActionListField> {

	createFormItem = (field: AutomationActionListField, data: IIndexer, options: DataFormOptions): React.ReactNode => {
		return <AutomationActionList key={field.key} field={field} options={options} />;
	};

	createEditNode(field: AutomationActionListField, data: IIndexer): React.ReactElement {
		return null;
	}

	createViewNode(field: AutomationActionListField, data: IIndexer): React.ReactElement {
		return null;
	}
}

export class AutomationItemProps {
	item: FieldData;
	typeSelector: React.ReactElement;
	options: DataFormOptions;
}

export abstract class AutomationConditionFactory<TCondition extends AutomationCondition> {
	private static Map: { [key: string]: AutomationConditionFactory<AutomationCondition>; } = {};

	static register(key: string, factory: AutomationConditionFactory<AutomationCondition>) {
		AutomationConditionFactory.Map[key] = factory;
	}

	static get(key: string): AutomationConditionFactory<AutomationCondition> {
		return AutomationConditionFactory.Map[key];
	}

	abstract createFormItem(condition: TCondition, props: AutomationItemProps): React.ReactNode;
}

export abstract class AutomationActionFactory<TAction extends AutomationAction> {
	private static Map: { [key: string]: AutomationActionFactory<AutomationAction>; } = {};

	static register(key: string, factory: AutomationActionFactory<AutomationAction>) {
		AutomationActionFactory.Map[key] = factory;
	}

	static get(key: string): AutomationActionFactory<AutomationAction> {
		return AutomationActionFactory.Map[key];
	}

	abstract createFormItem(action: TAction, props: AutomationItemProps): React.ReactNode;
}

export class GroupAutomationConditionFactory extends AutomationConditionFactory<GroupAutomationCondition> {
	createFormItem(condition: GroupAutomationCondition, props: AutomationItemProps): React.ReactElement {
		return <GroupAutomationConditionItem condition={condition} {...props} />;
	}
}

export class FieldAutomationConditionFactory extends AutomationConditionFactory<FieldAutomationCondition> {
	createFormItem(condition: FieldAutomationCondition, props: AutomationItemProps): React.ReactElement {
		return <FieldAutomationConditionItem condition={condition} {...props} />;
	}
}

export class SetFieldAutomationActionFactory extends AutomationConditionFactory<SetFieldAutomationAction> {
	createFormItem(action: SetFieldAutomationAction, props: AutomationItemProps): React.ReactElement {
		return <SetFieldAutomationActionItem action={action} {...props} />;
	}
}

export class NotifyByEmailAutomationActionFactory extends AutomationConditionFactory<NotifyByEmailAutomationAction> {
	createFormItem(action: NotifyByEmailAutomationAction, props: AutomationItemProps): React.ReactElement {
		return <NotifyByEmailAutomationActionItem action={action} {...props} />;
	}
}
