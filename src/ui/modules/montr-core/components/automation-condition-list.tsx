import React from "react";
import { Divider, Form, Typography, Space, Select, Button } from "antd";
import { Icon, ButtonAdd } from ".";
import { IAutomationConditionListField } from "@montr-core/models";

interface IProps {
	field: IAutomationConditionListField;
}

interface IState {
}

export class AutomationConditionList extends React.Component<IProps, IState> {

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
									<Select placeholder="Select condition">
										<Select.Option value="field">Document Status</Select.Option>
									</Select>
								</Form.Item>
								<Form.Item
									{...field}
									name={[field.name, "operator"]}
									fieldKey={[field.fieldKey, "operator"]}
									rules={[{ required: true }]}>
									<Select>
										<Select.Option value="Equal">Equal</Select.Option>
										<Select.Option value="NotEqual">NotEqual</Select.Option>
										<Select.Option value="LessThan">LessThan</Select.Option>
										<Select.Option value="LessThanEqual">LessThanEqual</Select.Option>
										<Select.Option value="GreaterThan">GreaterThan</Select.Option>
										<Select.Option value="GreaterThanEqual">GreaterThanEqual</Select.Option>
									</Select>
								</Form.Item>
								<Form.Item
									{...field}
									name={[field.name, "value"]}
									fieldKey={[field.fieldKey, "value"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select status">
										<Select.Option value="draft">Draft</Select.Option>
										<Select.Option value="published">Published</Select.Option>
										<Select.Option value="completed">Completed</Select.Option>
										<Select.Option value="closed">Closed</Select.Option>
									</Select>
								</Form.Item>

								<Button type="link" icon={Icon.MinusCircle}
									onClick={() => remove(field.name)} />
								<Button type="link" icon={Icon.ArrowUp} disabled={index == 0}
									onClick={() => move(index, index - 1)} />
								<Button type="link" icon={Icon.ArrowDown} disabled={index == array.length - 1}
									onClick={() => move(index, index + 1)} />

							</Space>
						))}

						<Form.Item>
							<ButtonAdd type="dashed" onClick={() => add()}>Add condition</ButtonAdd>
						</Form.Item>
					</>);
				}}
			</Form.List>

		);
	};
}
