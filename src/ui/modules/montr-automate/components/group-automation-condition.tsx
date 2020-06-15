import React from "react";
import { Radio, Form, Space } from "antd";
import { IGroupAutomationCondition } from "../models";
import { IAutomationConditionProps } from ".";

interface IProps extends IAutomationConditionProps {
	condition: IGroupAutomationCondition;
}

export class GroupAutomationCondition extends React.Component<IProps> {

	render = () => {
		const { typeSelector, item } = this.props;

		return (
			<Space style={{ display: "flex" }} align="start">

				{typeSelector}

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

			</Space>
		);
	};
}
