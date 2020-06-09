import React from "react";
import { Space, Input, Form } from "antd";
import { INotifyByEmailAutomationAction } from "../models";
import { IAutomationActionProps } from "./automation-field-factory";

interface IProps extends IAutomationActionProps {
	action: INotifyByEmailAutomationAction;
}

export class NotifyByEmailAutomationAction extends React.Component<IProps> {

	render = () => {
		const { typeSelector, item } = this.props;

		return (
			<Space style={{ display: "flex" }} align="start">

				{typeSelector}

				<Form.Item
					{...item}
					name={[item.name, "recipient"]}
					fieldKey={[item.fieldKey, "recipient"]}
					rules={[{ required: true }]}>
					<Input placeholder="Recipient" />
				</Form.Item>

			</Space>
		);
	};
}
