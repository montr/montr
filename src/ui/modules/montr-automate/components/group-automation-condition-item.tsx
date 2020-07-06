import React from "react";
import { Radio, Form, Space } from "antd";
import { GroupAutomationCondition, AutomationConditionListField } from "../models";
import { AutomationItemProps } from ".";
import { DataFieldFactory } from "@montr-core/components";

interface Props extends AutomationItemProps {
	// todo: rename to value here in other conditions and actions?
	condition: GroupAutomationCondition;
}

export class GroupAutomationConditionItem extends React.Component<Props> {

	render = () => {
		const { condition, options, typeSelector, item } = this.props;

		const field: AutomationConditionListField = {
			type: "automation-condition-list",
			key: "conditions"
		};

		const factory = DataFieldFactory.get(field.type);
		const children = factory.createFormItem(field, condition.conditions, options);

		return (<>
			<Space align="start">

				{typeSelector}

				<Form.Item
					{...item}
					name={[item.name, "props", "meet"]}
					fieldKey={[item.fieldKey, "meet"]}
					rules={[{ required: true }]}>
					<Radio.Group buttonStyle="solid">
						<Radio.Button value="All">All</Radio.Button>
						<Radio.Button value="Any">Any</Radio.Button>
					</Radio.Group>
				</Form.Item>

				<div style={{ border: "1px dotted lightgray" }}>
					{children}
				</div>
			</Space>
		</>);
	};
}
