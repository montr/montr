import React from "react";
import { Divider, Form, Space, Select, Button } from "antd";
import { Icon, ButtonAdd } from ".";
import { IAutomationConditionField } from "../models";
import { Toolbar } from "./toolbar";

interface IProps {
	field: IAutomationConditionField;
}

interface IState {
}

export class AutomationCondition extends React.Component<IProps, IState> {

	render = () => {
		const { field } = this.props;

		return <code key={field.key}>AutomationCondition</code>;

		/* return (
			<Form.List name={field.key}>
				{(fields, { add, remove, move }) => {
					console.log("AutomationCondition", fields);

					return (<>

						<Divider orientation="left">{field.name}</Divider>

						{fields.map((field, index, array) => (
							<Space key={field.key} style={{ display: "flex" }} align="start">
								<Form.Item
									{...field}
									name={[field.name, "type"]}
									fieldKey={[field.fieldKey, "type"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select condition" style={{ minWidth: 100 }}>
										<Select.Option value="field">Document Status</Select.Option>
									</Select>
								</Form.Item>
								<Form.Item
									{...field}
									name={[field.name, "operator"]}
									fieldKey={[field.fieldKey, "operator"]}
									rules={[{ required: true }]}>
									<Select style={{ minWidth: 50 }}>
										<Select.Option value="Equal">=</Select.Option>
										<Select.Option value="NotEqual">&lt;&gt;</Select.Option>
										<Select.Option value="LessThan">&lt;</Select.Option>
										<Select.Option value="LessThanEqual">&lt;=</Select.Option>
										<Select.Option value="GreaterThan">&gt;</Select.Option>
										<Select.Option value="GreaterThanEqual">&gt;=</Select.Option>
									</Select>
								</Form.Item>
								<Form.Item
									{...field}
									name={[field.name, "value"]}
									fieldKey={[field.fieldKey, "value"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select status" style={{ minWidth: 100 }}>
										<Select.Option value="draft">Draft</Select.Option>
										<Select.Option value="published">Published</Select.Option>
										<Select.Option value="completed">Completed</Select.Option>
										<Select.Option value="closed">Closed</Select.Option>
									</Select>
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
							<ButtonAdd type="dashed" onClick={() => add()}>Add condition</ButtonAdd>
							<ButtonAdd type="dashed" onClick={() => add()}>Add group</ButtonAdd>
						</Toolbar>
					</>);
				}}
			</Form.List>

		); */
	};
}
