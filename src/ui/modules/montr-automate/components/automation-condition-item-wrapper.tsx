import React from "react";
import { AutomationConditionFactory, AutomationItemProps } from ".";
import { AutomationCondition } from "../models";
import { AutomationConditionItem } from "./automation-condition-item";

interface Props extends AutomationItemProps {
	value?: AutomationCondition;
}

export class AutomationConditionItemWrapper extends React.Component<Props> {

	render = () => {
		const { value } = this.props;

		if (value?.type) {
			const factory = AutomationConditionFactory.get(value?.type);

			if (factory) {
				return factory.createFormItem(value, { ...this.props });
			}
		}

		return <AutomationConditionItem condition={value} {...this.props} />;
	};
}
