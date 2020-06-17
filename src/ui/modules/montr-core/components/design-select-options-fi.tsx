import React from "react";
import { Input, Form, Space } from "antd";
import { IDesignSelectOptionsField } from "../models";
import { ButtonAdd, FormListItemToolbar, Toolbar } from ".";

interface IProps {
	field: IDesignSelectOptionsField;
}

interface IState {
}

// todo: consider using Dynamic Form Item (Form.List component)
// https://next.ant.design/components/form/#components-form-demo-dynamic-form-item
// or using table with inline editing
// https://next.ant.design/components/table/#components-table-demo-edit-cell
export class DesignSelectOptionsFI extends React.Component<IProps, IState> {

	render = () => {
		const { field } = this.props;

		return (
			<Form.List name={field.key}>
				{(items, { add, remove, move }) => {
					return (<>

						{items.map((item, index) => {

							return (
								<div key={item.key}>

									<FormListItemToolbar
										item={item}
										itemIndex={index}
										itemsCount={items.length}
										ops={{ remove, move }} />

									<Space style={{ display: "flex" }} align="start">

										<Form.Item
											{...item}
											name={[item.name, "name"]}
											// fieldKey={[item.fieldKey, "name"]}
											rules={[{ required: true }]}>
											<Input placeholder="Name" />
										</Form.Item>

										<Form.Item
											{...item}
											name={[item.name, "value"]}
											// fieldKey={[item.fieldKey, "value"]}
											rules={[{ required: true }]}>
											<Input placeholder="Value" />
										</Form.Item>

									</Space>
								</div>
							);

						})}


						<Toolbar>
							<ButtonAdd type="dashed" onClick={() => add({ conditions: [] })} />
						</Toolbar>
					</>);
				}}
			</Form.List>
		);
	};
}
