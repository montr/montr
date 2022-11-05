import { ButtonAdd, DataFormOptions, Toolbar } from "@montr-core/components";
import { FormListItemToolbar } from "@montr-core/components/form-list-item-toolbar";
import { Divider, Form, Select } from "antd";
import React from "react";
import { AutomationActionItemWrapper } from ".";
import { AutomationActionListField, AutomationRuleType } from "../models";
import { AutomationService } from "../services";

interface Props {
	field: AutomationActionListField;
	options: DataFormOptions;
}

interface State {
	loading: boolean;
	types: AutomationRuleType[];
}

export class AutomationActionList extends React.Component<Props, State> {

	private readonly automationService = new AutomationService();

	constructor(props: Props) {
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
		this.setState({ loading: false, types: await this.automationService.actionTypes() });
	};

	render = () => {
		const { field, options } = this.props,
			{ types } = this.state;

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
									/* fieldKey={[item.fieldKey, "type"]} */
									rules={[{ required: true }]}>
									<Select placeholder="Select action" style={{ minWidth: 150 }}>
										{types.map(x => <Select.Option key={x.code} value={x.code}>{x.name}</Select.Option>)}
									</Select>
								</Form.Item>
							);

							return (
								<div key={item.key + '_' + index}>

									<FormListItemToolbar
										item={item}
										itemIndex={index}
										itemsCount={items.length}
										ops={{ remove, move }} />

									<Form.Item
										{...item}
										name={[item.name]}
										/* fieldKey={[item.fieldKey]} */
										rules={[{ required: true }]}
										noStyle>
										<AutomationActionItemWrapper item={item} typeSelector={typeSelector} options={options} />
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
