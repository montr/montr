import React, { ChangeEvent } from "react";
import { Input, Row, Col, Typography, Button } from "antd";
import { Gutter } from "antd/lib/grid/row";
import { IOption, Guid, IIndexer } from "../models";
import { ButtonAdd } from ".";
import { Icon } from "./icon";

interface IProps {
	value?: IOption[];
	onChange?: (value: any) => void;
}

interface IState {
	options?: IOption[];
	items: IItem[];
}

interface IItem extends IIndexer {
	key: Guid;
	value: string;
	name: string;
}

export class DesignSelectOptions extends React.Component<IProps, IState> {

	/* static getDerivedStateFromProps(nextProps: IProps, prevState: IState) {
		console.log("getDerivedStateFromProps", nextProps, prevState);
		if (nextProps.value !== prevState.options) {
			return nextProps.value ?? null;
		}
		return null;
	} */

	constructor(props: IProps) {
		super(props);

		const items: IItem[] = (props.value ?? []).map(x => {
			return { name: x.name, value: x.value, key: Guid.newGuid() };
		});

		this.state = {
			options: props.value,
			items: items
		};
	}

	/* shouldComponentUpdate(nextProps: IProps, nextState: IState, nextContext: any): boolean {
		return this.props.value !== nextProps.value;
	} */

	handleChange = (items: IItem[]) => {
		const { onChange } = this.props;

		if (onChange) {
			const options = items.map(x => {
				return { name: x.name, value: x.value };
			});

			onChange(options);
		}
	};

	addOption = () => {
		const { items } = this.state;

		const num = items.length + 1,
			newItem = { name: "Option " + num, value: "value" + num, key: Guid.newGuid() };

		this.handleChange([...items, newItem]);
	};

	removeOption = (key: Guid) => {
		const items = this.state.items.filter(x => {
			return x.key != key;
		});

		this.handleChange(items);
	};

	changeItemProp = (item: IItem, e: ChangeEvent<HTMLInputElement>) => {
		const { items } = this.state;

		if (item[e.target.name] != e.target.value) {
			item[e.target.name] = e.target.value;

			this.handleChange(items);
		}
	};

	render = () => {
		const { items } = this.state;

		const gutter: [Gutter, Gutter] = [{ xs: 4, sm: 8, md: 12, lg: 16 }, 4];

		console.log("render");

		return (<>
			<div>
				<Row gutter={gutter}>
					<Col span={8}><Typography.Text>Name</Typography.Text></Col>
					<Col span={8}><Typography.Text>Value</Typography.Text></Col>
				</Row>

				{items.map(x => <Row gutter={gutter} key={x.key.toString()}>
					<Col span={8}>
						<Input name="name" value={x.name} onChange={(e) => this.changeItemProp(x, e)} />
					</Col>
					<Col span={8}>
						<Input name="value" value={x.value} onChange={(e) => this.changeItemProp(x, e)} />
					</Col>
					<Col span={8}>
						<Button type="link" icon={Icon.MinusCircle} onClick={() => this.removeOption(x.key)} />
						<Button type="link" icon={Icon.ArrowUp} />
						<Button type="link" icon={Icon.ArrowDown} />
					</Col>
				</Row>)}
			</div>

			<ButtonAdd onClick={this.addOption} />
		</>);
	};
}
