import React, { ChangeEvent } from "react";
import { Input, Row, Col, Typography, Button } from "antd";
import { Gutter } from "antd/lib/grid/row";
import { IOption, IIndexer, IDesignSelectOptionsField } from "../models";
import { ButtonAdd } from ".";
import { Icon } from "./icon";

interface Props {
	field: IDesignSelectOptionsField;
	value?: IOption[];
	onChange?: (value: any) => void;
}

interface State {
	options?: IOption[];
}

// todo: consider using Dynamic Form Item (Form.List component)
// https://next.ant.design/components/form/#components-form-demo-dynamic-form-item
// or using table with inline editing
// https://next.ant.design/components/table/#components-table-demo-edit-cell
export class DesignSelectOptions extends React.Component<Props, State> {

	/* static getDerivedStateFromProps(nextProps: IProps, prevState: IState) {
		console.log("getDerivedStateFromProps", nextProps, prevState);
		if (nextProps.value !== prevState.options) {
			return nextProps.value ?? null;
		}
		return null;
	} */

	constructor(props: Props) {
		super(props);

		this.state = {
			options: props.value
		};
	}

	componentDidUpdate = async (prevProps: Props) => {
		if (this.props.value !== prevProps.value) {
			this.setState({ options: this.props.value });
		}
	};

	/* shouldComponentUpdate(nextProps: IProps, nextState: IState, nextContext: any): boolean {
		return this.props.value !== nextProps.value;
	} */

	handleChange = (items: IOption[]) => {
		const { onChange } = this.props;

		if (onChange) {
			onChange(items);
		}
	};

	addOption = () => {
		const { options } = this.state;

		this.handleChange([...options ?? [], { name: "", value: "" }]);
	};

	removeOption = (index: number) => {
		const { options } = this.state;

		options.splice(index, 1);

		this.handleChange([...options]);
	};

	swapOptions = (index1: number, index2: number) => {
		const { options } = this.state;

		[options[index2], options[index1]] = [options[index1], options[index2]];

		this.handleChange([...options]);
	};

	changeItemProp = (option: IOption, e: ChangeEvent<HTMLInputElement>) => {
		const item = option as IIndexer;

		if (item[e.target.name] != e.target.value) {
			const { options } = this.state;

			item[e.target.name] = e.target.value;

			this.handleChange([...options]);
		}
	};

	render = () => {
		const { options } = this.state,
			count = options?.length;

		const gutter: [Gutter, Gutter] = [{ xs: 4, sm: 8, md: 12, lg: 16 }, 4];

		return (<>
			<div>
				<Row gutter={gutter}>
					<Col span={8}><Typography.Text>Name</Typography.Text></Col>
					<Col span={8}><Typography.Text>Value</Typography.Text></Col>
				</Row>

				{options?.map((item, index) => <Row gutter={gutter} key={index}>
					<Col span={8}>
						<Input name="name" placeholder="Name" value={item.name} onChange={(e) => this.changeItemProp(item, e)} />
					</Col>
					<Col span={8}>
						<Input name="value" placeholder="Value" value={item.value} onChange={(e) => this.changeItemProp(item, e)} />
					</Col>
					<Col span={8}>
						{count > 1 && <>
							<Button type="link" icon={Icon.MinusCircle}
								onClick={() => this.removeOption(index)} />
							<Button type="link" icon={Icon.ArrowUp}
								disabled={index == 0}
								onClick={() => this.swapOptions(index, index - 1)} />
							<Button type="link" icon={Icon.ArrowDown}
								disabled={index == count - 1}
								onClick={() => this.swapOptions(index, index + 1)} />
						</>}
					</Col>
				</Row>)}
			</div>

			<ButtonAdd type="dashed" onClick={this.addOption} />
		</>);
	};
}
