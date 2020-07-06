import React from "react";
import { Space, Input, Form, Select } from "antd";
import { AutomationItemProps } from "./automation-field-factory";
import { SetFieldAutomationAction } from "../models/automation";

interface Props extends AutomationItemProps {
	action: SetFieldAutomationAction;
}

export class SetFieldAutomationActionItem extends React.Component<Props> {

	render = () => {
		const { typeSelector, item } = this.props;

		return (
			<Space align="start">

				{typeSelector}

				<Form.Item
					{...item}
					name={[item.name, "field"]}
					fieldKey={[item.fieldKey, "field"]}
					rules={[{ required: true }]}>
					<Select placeholder="Field" />
				</Form.Item>

				<Form.Item
					{...item}
					name={[item.name, "value"]}
					fieldKey={[item.fieldKey, "value"]}
					rules={[{ required: true }]}>
					<Input placeholder="Value" />
				</Form.Item>

			</Space>
		);
	};
}
