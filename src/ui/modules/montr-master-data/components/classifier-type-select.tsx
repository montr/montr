import React from "react";
import { Select } from "antd";
import { IClassifierType, IClassifierTypeField } from "../models";
import { ClassifierTypeService } from "../services";

interface IProps {
	value?: string;
	field: IClassifierTypeField;
	onChange?: (value: any) => void;
}

interface IState {
	loading: boolean;
	types?: IClassifierType[];
}

export class ClassifierTypeSelect extends React.Component<IProps, IState> {

	private _classifierTypeService = new ClassifierTypeService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
	};

	fetchData = async () => {
		const types = await this._classifierTypeService.list({ skipPaging: true });

		this.setState({ loading: false, types: types.rows });
	};

	handleChange = (value: any) => {
		const { onChange } = this.props;

		if (onChange) {
			onChange(value);
		}
	};

	render = () => {
		const { value } = this.props,
			{ loading, types } = this.state;

		return (
			<Select
				value={value}
				loading={loading}
				showArrow={true}
				onChange={this.handleChange}>
				{types && types.map(x => <Select.Option key={x.code} value={x.code}>{x.name}</Select.Option>)}
			</Select>
		);
	};
}
