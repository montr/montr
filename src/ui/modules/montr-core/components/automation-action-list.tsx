import React from "react";
import { Divider, Form, Space, Select, Button, Input } from "antd";
import { Icon, ButtonAdd } from ".";
import { IAutomationActionListField } from "../models";
import { Toolbar } from "./toolbar";

interface IProps {
	field: IAutomationActionListField;
}

interface IState {
}

export class AutomationActionList extends React.Component<IProps, IState> {

	render = () => {
		const { field } = this.props;

		return (
			<Form.List name={field.key}>
				{(fields, { add, remove, move }) => {
					return (<>

						<Divider orientation="left">{field.name}</Divider>

						{fields.map((field, index, array) => (
							<Space key={field.key} style={{ display: "flex" }} align="start">
								<Form.Item
									{...field}
									name={[field.name, "type"]}
									fieldKey={[field.fieldKey, "type"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select action" style={{ minWidth: 100 }}>
										<Select.Option value="field">Send Email</Select.Option>
									</Select>
								</Form.Item>
								<Form.Item
									{...field}
									name={[field.name, "recipient"]}
									fieldKey={[field.fieldKey, "recipient"]}
									rules={[{ required: true }]}>
									<Input />
								</Form.Item>

								<div>
									<Button type="link" icon={Icon.MinusCircle} size="small"
										onClick={() => remove(field.name)} />
									<Button type="link" icon={Icon.ArrowUp} size="small" disabled={index == 0}
										onClick={() => move(index, index - 1)} />
									<Button type="link" icon={Icon.ArrowDown} size="small" disabled={index == array.length - 1}
										onClick={() => move(index, index + 1)} />
								</div>

							</Space>
						))}

						<Toolbar>
							<ButtonAdd type="dashed" onClick={() => add()}>Add action</ButtonAdd>
						</Toolbar>
					</>);
				}}
			</Form.List>

		);
	};
}
