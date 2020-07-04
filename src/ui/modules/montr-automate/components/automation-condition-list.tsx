import React from "react";
import { Divider, Form, Select } from "antd";
import { ButtonAdd, Toolbar, FormListItemToolbar, IDataFormOptions } from "@montr-core/components";
import { IAutomationConditionListField, IFieldAutomationCondition } from "../models/";
import { AutomationCondition } from ".";

interface IProps {
	field: IAutomationConditionListField;
	options: IDataFormOptions;
}

export class AutomationConditionList extends React.Component<IProps> {

	render = () => {
		const { field, options } = this.props;

		const defaultCondition: IFieldAutomationCondition = { type: "field", operator: "Equal" };

		return (
			<Form.List name={field.key}>
				{(items, { add, remove, move }) => {

					return (<>

						{field.name && <Divider orientation="left">{field.name}</Divider>}

						{items.map((item, index) => {

							const typeSelector = (
								<Form.Item
									{...item}
									name={[item.name, "type"]}
									fieldKey={[item.fieldKey, "type"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select condition" style={{ minWidth: 150 }}>
										<Select.Option value="group">Group</Select.Option>
										<Select.Option value="field">Field</Select.Option>
									</Select>
								</Form.Item>
							);

							return (
								<div key={item.key}>

									<FormListItemToolbar
										item={item}
										itemIndex={index}
										itemsCount={items.length}
										ops={{ remove, move }} />

									<Form.Item
										{...item}
										name={[item.name]}
										fieldKey={[item.fieldKey]}
										rules={[{ required: true }]}
										noStyle>
										<AutomationCondition item={item} typeSelector={typeSelector} options={options} />
									</Form.Item>

								</div>
							);
						})}

						<Toolbar>
							<ButtonAdd type="dashed" onClick={() => add(defaultCondition)}>Add condition</ButtonAdd>
						</Toolbar>
					</>);
				}}
			</Form.List>
		);
	};
}
