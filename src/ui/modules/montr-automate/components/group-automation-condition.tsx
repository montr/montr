import React from "react";
import { Radio, Form, Space, Select } from "antd";
import { IGroupAutomationCondition, IAutomationConditionListField } from "../models";
import { IAutomationItemProps } from ".";
import { DataFieldFactory, IDataFormOptions } from "@montr-core/components";

interface IProps extends IAutomationItemProps {
	// todo: rename to value here in other conditions and actions?
	condition: IGroupAutomationCondition;
}

export class GroupAutomationCondition extends React.Component<IProps> {

	render = () => {
		const { condition, options, typeSelector, item } = this.props;

		const field: IAutomationConditionListField = {
			type: "automation-condition-list",
			key: "conditions"
		};

		const factory = DataFieldFactory.get(field.type);
		const children = factory.createFormItem(field, condition.conditions, options);

		return (<>
			<Space style={{ display: "flex" }} align="start">

				{/* {typeSelector} */}

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

				<Form.Item
					{...item}
					name={[item.name, "meet"]}
					fieldKey={[item.fieldKey, "meet"]}
					rules={[{ required: true }]}>
					<Radio.Group buttonStyle="solid">
						<Radio.Button value="all">All</Radio.Button>
						<Radio.Button value="any">Any</Radio.Button>
					</Radio.Group>
				</Form.Item>

				<div style={{ border: "1px solid gray" }}>
					{children}
				</div>
			</Space>
		</>);
	};
}
