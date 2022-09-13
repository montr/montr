import { DataFieldFactory } from "@montr-core/components";
import { IDataField } from "@montr-core/models";
import { Form, Select, Space } from "antd";
import React from "react";
import { AutomationContextProps, AutomationItemProps, withAutomationContext } from ".";
import { FieldAutomationCondition } from "../models";
import { FieldAutomationConditionService } from "../services";

interface Props extends AutomationItemProps, AutomationContextProps {
	condition: FieldAutomationCondition;
}

interface State {
	fields: IDataField[];
	selectedField?: IDataField;
}

class WrappedFieldAutomationConditionItem extends React.Component<Props, State> {

	private readonly automationConditionService = new FieldAutomationConditionService();

	constructor(props: Props) {
		super(props);

		this.state = {
			fields: []
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this.automationConditionService.abort();
	};

	fetchData = async () => {
		const { data } = this.props;

		if (data) {
			const fields = await this.automationConditionService.fields(data.entityTypeCode, data.Uid);

			this.setState({ fields });
		}
	};

	onFieldChange = async (value: any, option: any) => {
		this.setState({ selectedField: option.field });
	};

	render = (): React.ReactNode => {
		const { typeSelector, item, options } = this.props,
			{ fields, selectedField } = this.state;

		let valueComponent: React.ReactNode;

		if (selectedField) {
			const factory = DataFieldFactory.get(selectedField.type);
			valueComponent = factory?.createFormItem(selectedField, item, { hideLabels: true, ...options });
		}

		return (
			<Space align="start">

				{typeSelector}

				<Form.Item
					{...item}
					name={[item.name, "props", "field"]}
					/* fieldKey={[item.fieldKey, "field"]} */
					rules={[{ required: true }]}>
					<Select placeholder="Select field" style={{ minWidth: 200 }}
						onChange={this.onFieldChange}>
						{fields?.map(x =>
							<Select.Option key={x.key} value={x.key} field={x}>{x.name}</Select.Option>)}
					</Select>
				</Form.Item>

				<Form.Item
					{...item}
					name={[item.name, "props", "operator"]}
					/* fieldKey={[item.fieldKey, "operator"]} */
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
					{...item}
					name={[item.name, "props", "value"]}
					/* fieldKey={[item.fieldKey, "value"]} */
					rules={[{ required: true }]}>

					{valueComponent}

					{/* <Select placeholder="Select value" style={{ minWidth: 100 }}>
						<Select.Option value="draft">Draft</Select.Option>
						<Select.Option value="submitted">Submitted</Select.Option>
						<Select.Option value="published">Published</Select.Option>
						<Select.Option value="completed">Completed</Select.Option>
						<Select.Option value="closed">Closed</Select.Option>
					</Select> */}
				</Form.Item>

			</Space>
		);
	};
}

export const FieldAutomationConditionItem = withAutomationContext(WrappedFieldAutomationConditionItem);
