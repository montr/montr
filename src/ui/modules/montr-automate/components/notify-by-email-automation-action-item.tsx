import React from "react";
import { Space, Input, Form } from "antd";
import { AutomationItemProps } from "./automation-field-factory";
import { NotifyByEmailAutomationAction } from "../models/automation";
import { FormDefaults } from "@montr-core/components";

interface Props extends AutomationItemProps {
	action: NotifyByEmailAutomationAction;
}

export class NotifyByEmailAutomationActionItem extends React.Component<Props> {

	render = () => {
		const { typeSelector, item } = this.props;

		const { key, ...other } = item;
		const itemProps = { /* ...other, */ ...FormDefaults.formItemLayout };

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
