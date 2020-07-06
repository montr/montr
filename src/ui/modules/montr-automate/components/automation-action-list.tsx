import React from "react";
import { Divider, Form, Select } from "antd";
import { ButtonAdd, Toolbar, FormListItemToolbar, IDataFormOptions } from "@montr-core/components";
import { AutomationActionListField, AutomationRuleType } from "../models";
import { AutomationActionItemWrapper } from ".";
import { AutomationService } from "../services";

interface Props {
	field: AutomationActionListField;
	options: IDataFormOptions;
}

interface State {
	loading: boolean;
	types: AutomationRuleType[];
}

export class AutomationActionList extends React.Component<Props, State> {

	private _automationService = new AutomationService();

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
		this.setState({ loading: false, types: await this._automationService.actionTypes() });
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
									fieldKey={[item.fieldKey, "type"]}
									rules={[{ required: true }]}>
									<Select placeholder="Select action" style={{ minWidth: 150 }}>
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
