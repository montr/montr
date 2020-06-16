import React from "react";
import { Space, Input, Form, Select } from "antd";
import { IAutomationItemProps } from "./automation-field-factory";
import { ISetFieldAutomationAction } from "../models/automation";

interface IProps extends IAutomationItemProps {
	action: ISetFieldAutomationAction;
}

export class SetFieldAutomationAction extends React.Component<IProps> {

	render = () => {
		const { typeSelector, item } = this.props;

		return (
			<Space style={{ display: "flex" }} align="start">

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
