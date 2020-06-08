import React from "react";
import { Form, Space, Input } from "antd";

// from src\ui\node_modules\antd\lib\form\FormList.d.ts
interface FieldData {
	name: number;
	key: number;
	fieldKey: number;
}

interface IProps {
	typeSelector: React.ReactElement;
	item: FieldData;
	value?: string;
}

interface IState {
}

export class AutomationAction extends React.Component<IProps, IState> {

	constructor(props: IProps) {
		super(props);

		this.state = {
		};
	}

	render = () => {
		const { typeSelector, item } = this.props;

		return (
			<Space key={item.key} style={{ display: "flex" }} align="start">

				{typeSelector}

				<Form.Item
					{...item}
					name={[item.name, "value"]}
					fieldKey={[item.fieldKey, "value"]}
					rules={[{ required: true }]}>
					<Input />
				</Form.Item>

			</Space>
		);
	};
}
