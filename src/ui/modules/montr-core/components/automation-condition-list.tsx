import React from "react";
import { Divider, Form, Typography, Space, Select, Button } from "antd";
import { Icon, ButtonAdd } from ".";

interface IProps {
}

interface IState {
}

export class AutomationConditionList extends React.Component<IProps, IState> {

	onFinish = (values: any) => {
		console.log(values);
	};

	render = () => {
		return (<Form onFinish={this.onFinish} autoComplete="off">
			<Form.List name="conditions">
				{(fields, { add, remove, move }) => {
					return (<>

						<Divider>Meet <Typography.Text code>all</Typography.Text> of the following conditions</Divider>

						{fields.map((field, index, array) => (
							<Space key={field.key} style={{ display: "flex" }} align="start">
								<Form.Item
									{...field}
									name={[field.name, "type"]}
									fieldKey={[field.fieldKey, "type"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select condition">
										<Select.Option value="field">Status</Select.Option>
									</Select>
								</Form.Item>
								<Form.Item
									{...field}
									name={[field.name, "value"]}
									fieldKey={[field.fieldKey, "value"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select status">
										<Select.Option value="draft">draft</Select.Option>
										<Select.Option value="published">published</Select.Option>
										<Select.Option value="completed">completed</Select.Option>
										<Select.Option value="closed">closed</Select.Option>
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
			<Form.Item>
				<Button type="primary" htmlType="submit">Submit</Button>
			</Form.Item>
		</Form>
		);
	};
}
