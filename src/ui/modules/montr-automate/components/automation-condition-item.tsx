import React from "react";
import { Space } from "antd";
import { AutomationConditionFactory, AutomationItemProps } from ".";
import { AutomationCondition } from "../models";

interface Props extends AutomationItemProps {
	value?: AutomationCondition;
}

export class AutomationConditionItem extends React.Component<Props> {

	render = () => {
		const { value, typeSelector } = this.props;

		if (value?.type) {
			const factory = AutomationConditionFactory.get(value?.type);

			if (factory) {
				const control = factory.createFormItem(value, { ...this.props });

				return control;
			}
			else {
				console.error(`Automation condition type ${value.type} is not found.`);
			}
		}

		return (
			<Space style={{ display: "flex" }} align="start">
				{typeSelector}
			</Space>
		);
	};
}
