import React from "react";
import { Space } from "antd";
import { AutomationItemProps, AutomationActionFactory } from ".";
import { AutomationAction } from "../models/automation";

interface Props extends AutomationItemProps {
	value?: AutomationAction;
}

export class AutomationActionItemWrapper extends React.Component<Props> {

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
