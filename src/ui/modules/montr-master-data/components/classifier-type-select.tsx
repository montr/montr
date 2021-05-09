import React from "react";
import { Select } from "antd";
import { ClassifierType, IClassifierTypeField } from "../models";
import { ClassifierTypeService } from "../services";

interface Props {
	value?: string;
	field: IClassifierTypeField;
	onChange?: (value: any) => void;
}

interface State {
	loading: boolean;
	types?: ClassifierType[];
}

export class ClassifierTypeSelect extends React.Component<Props, State> {

	private _classifierTypeService = new ClassifierTypeService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this._classifierTypeService.abort();
	};

	fetchData = async (): Promise<void> => {
		const types = await this._classifierTypeService.list({ skipPaging: true });

		this.setState({ loading: false, types: types.rows });
	};

	handleChange = (value: any): void => {
		const { onChange } = this.props;

		if (onChange) {
			onChange(value);
		}
	};

	render = (): React.ReactNode => {
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
