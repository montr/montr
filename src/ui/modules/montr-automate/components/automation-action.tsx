import React from "react";
import { Space } from "antd";
import { IAutomationActionProps, AutomationActionFactory } from ".";
import { IAutomationAction } from "../models/automation";

interface IActionProps extends IAutomationActionProps {
	value?: IAutomationAction;
}

export class AutomationAction extends React.Component<IActionProps> {

	render = () => {
		const { value, typeSelector } = this.props;

		if (value?.type) {
			const factory = AutomationActionFactory.get(value?.type);

			if (factory) {
				const control = factory.createFormItem(value, { ...this.props });

				return control;
			}
			else {
				console.error(`Automation action type ${value.type} is not found.`);
			}
		}

		return (
			<Space style={{ display: "flex" }} align="start">
				{typeSelector}
			</Space>
		);
	};
}

