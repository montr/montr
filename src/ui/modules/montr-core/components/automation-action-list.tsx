import React from "react";
import { Divider, Form, Select, Button } from "antd";
import { Icon, ButtonAdd } from ".";
import { IAutomationActionListField } from "../models";
import { Toolbar } from "./toolbar";
import { AutomationAction } from "./automation-action";

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
				{(items, { add, remove, move }) => {

					return (<>

						<Divider orientation="left">{field.name}</Divider>

						{items.map((item, index, array) => {

							const typeSelector = (
								<Form.Item
									{...item}
									name={[item.name, "type"]}
									fieldKey={[item.fieldKey, "type"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select action" style={{ minWidth: 150 }}>
										<Select.Option value="set-field">Set Field</Select.Option>
										<Select.Option value="notify-by-email">Notify By Email</Select.Option>
									</Select>
								</Form.Item>
							);

							const itemToolbar = (
								<Toolbar float="right">
									<Button type="link" icon={Icon.MinusCircle}
										onClick={() => remove(item.name)} />
									<Button type="link" icon={Icon.ArrowUp} disabled={index == 0}
										onClick={() => move(index, index - 1)} />
									<Button type="link" icon={Icon.ArrowDown} disabled={index == array.length - 1}
										onClick={() => move(index, index + 1)} />
								</Toolbar>
							);

							return (
								<div>
									{itemToolbar}

									<AutomationAction item={item}
										typeSelector={typeSelector}
									/>

								</div>
							);
						})}

						<Toolbar>
							<ButtonAdd type="dashed" onClick={() => add()}>Add action</ButtonAdd>
						</Toolbar>
					</>);
				}}
			</Form.List>

		);
	};
}
