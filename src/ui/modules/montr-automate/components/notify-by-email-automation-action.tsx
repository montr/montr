import React from "react";
import { Space, Input, Form } from "antd";
import { IAutomationItemProps } from "./automation-field-factory";
import { INotifyByEmailAutomationAction } from "../models/automation";
import { FormDefaults } from "@montr-core/components";

interface IProps extends IAutomationItemProps {
	action: INotifyByEmailAutomationAction;
}

export class NotifyByEmailAutomationAction extends React.Component<IProps> {

	render = () => {
		const { typeSelector, item } = this.props;

		const itemProps = { ...item, ...FormDefaults.formItemLayout };
		return (<>
			<Space align="start">
				{typeSelector}
			</Space>

			<Form.Item
				{...itemProps}
				label="Recipient"
				name={[item.name, "props", "recipient"]}
				fieldKey={[item.fieldKey, "recipient"]}
				rules={[{ required: true }]}>
				<Input placeholder="Recipient" />
			</Form.Item>

			<Form.Item
				{...itemProps}
				label="Subject"
				name={[item.name, "props", "subject"]}
				fieldKey={[item.fieldKey, "subject"]}
				rules={[{ required: true }]}>
				<Input placeholder="Subject" />
			</Form.Item>

			<Form.Item
				{...itemProps}
				label="Body"
				name={[item.name, "props", "body"]}
				fieldKey={[item.fieldKey, "body"]}
				rules={[{ required: true }]}>
				<Input.TextArea placeholder="Body" rows={4} />
			</Form.Item>

		</>);
	};
}
