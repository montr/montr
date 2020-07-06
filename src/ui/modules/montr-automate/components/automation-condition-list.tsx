import React from "react";
import { Divider, Form, Select } from "antd";
import { ButtonAdd, Toolbar, FormListItemToolbar, IDataFormOptions } from "@montr-core/components";
import { IAutomationConditionListField, IFieldAutomationCondition, AutomationRuleType } from "../models/";
import { AutomationCondition } from ".";
import { AutomationService } from "../services";

interface IProps {
	field: IAutomationConditionListField;
	options: IDataFormOptions;
}

interface IState {
	loading: boolean;
	types: AutomationRuleType[];
}

export class AutomationConditionList extends React.Component<IProps, IState> {

	private _automationService = new AutomationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			types: []
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	fetchData = async () => {
		this.setState({ loading: false, types: await this._automationService.conditionTypes() });
	};

	render = () => {
		const { field, options } = this.props,
			{ types } = this.state;

		const defaultCondition/* : IFieldAutomationCondition */ = { type: "field", props: { operator: "Equal" } };

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
										{types.map(x => <Select.Option key={x.code} value={x.code}>{x.name}</Select.Option>)}
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
