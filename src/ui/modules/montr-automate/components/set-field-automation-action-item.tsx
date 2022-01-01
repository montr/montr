import { Form, Input, Select, Space } from "antd";
import React from "react";
import { SetFieldAutomationAction } from "../models/automation";
import { AutomationItemProps } from "./automation-field-factory";

interface Props extends AutomationItemProps {
	action: SetFieldAutomationAction;
}

export class SetFieldAutomationActionItem extends React.Component<Props> {

	render = (): React.ReactNode => {
		const { typeSelector, item } = this.props;

		return (
			<Space align="start">

				{typeSelector}

				<Form.Item
					{...item}
					name={[item.name, "field"]}
					/* fieldKey={[item.fieldKey, "field"]} */
					rules={[{ required: true }]}>
					<Select placeholder="Field" />
				</Form.Item>

				<Form.Item
					{...item}
					name={[item.name, "value"]}
					/* fieldKey={[item.fieldKey, "value"]} */
					rules={[{ required: true }]}>
					<Input placeholder="Value" />
				</Form.Item>

			</Space>
		);
	};
}
