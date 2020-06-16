import React from "react";
import { Space } from "antd";
import { AutomationConditionFactory, IAutomationItemProps } from ".";
import { IAutomationCondition } from "../models";

interface IProps extends IAutomationItemProps {
	value?: IAutomationCondition;
}

export class AutomationCondition extends React.Component<IProps> {

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
