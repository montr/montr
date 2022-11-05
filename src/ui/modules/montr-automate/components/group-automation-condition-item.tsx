import { DataFormOptions, extendNamePath } from "@montr-core/components";
import { DataFieldFactory } from "@montr-core/components/data-field-factory";
import { Form, Radio, Space } from "antd";
import React from "react";
import { AutomationItemProps } from ".";
import { AutomationConditionListField, GroupAutomationCondition } from "../models";

interface Props extends AutomationItemProps {
	// todo: rename to value here in other conditions and actions?
	condition: GroupAutomationCondition;
}

export class GroupAutomationConditionItem extends React.Component<Props> {

	render = () => {
		const { condition, options, typeSelector, item } = this.props;

		const field: Partial<AutomationConditionListField> = {
			type: "automation-condition-list",
			key: "conditions"
		};

		const innerOptions: DataFormOptions = {
			namePathPrefix: extendNamePath(item.name, ["props", "conditions"]),
			...options
		};

		const factory = DataFieldFactory.get(field.type);

		const children = factory?.createFormItem(field, null /* condition.conditions */, innerOptions);

		return (<>
			<Space align="start">

				{typeSelector}

				<Form.Item
					{...item}
					name={extendNamePath(item.name, ["props", "meet"])}
					/* fieldKey={[item.fieldKey, "meet"]} */
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
