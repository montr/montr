import React from "react";
import { Divider, Form, Select, Button, Space } from "antd";
import { Icon, ButtonAdd, Toolbar } from "@montr-core/components";
import { AutomationActionFactory, IAutomationActionProps } from "./automation-field-factory";
import { IAutomationActionListField } from "@montr-automate/models/automation-field";
import { IAutomationAction } from "@montr-automate/models/automation";

interface IProps {
	field: IAutomationActionListField;
}

export class AutomationActionList extends React.Component<IProps> {

	render = () => {
		const { field } = this.props;

		return (
			<Form.List name={field.key}>
				{(items, { add, remove, move }) => {

					return (<>

						<Divider orientation="left">{field.name}</Divider>

						{items.map((item, index) => {

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
									<Button type="link" icon={Icon.ArrowDown} disabled={index == items.length - 1}
										onClick={() => move(index, index + 1)} />
								</Toolbar>
							);

							return (
								<div key={item.key}>
									{itemToolbar}

									<Form.Item
										{...item}
										name={[item.name]}
										fieldKey={[item.fieldKey]}
										rules={[{ required: true }]}
										noStyle>
										<AutomationAction item={item} typeSelector={typeSelector} />
									</Form.Item>
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

interface IActionProps extends IAutomationActionProps {
	value?: IAutomationAction;
}

class AutomationAction extends React.Component<IActionProps> {

	render = () => {
		const { value, typeSelector } = this.props;

		if (value?.type) {
			const factory = AutomationActionFactory.get(value?.type);

			if (factory) {
				const control = factory.createFormItem(value, { ...this.props });

				return control;
			}
			else {
				console.error(`Automation action type ${value.type} is not found.`);
			}
		}

		return (
			<Space style={{ display: "flex" }} align="start">
				{typeSelector}
			</Space>
		);
	};
}

