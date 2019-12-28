import React from "react";
import { DataForm } from ".";
import { MetadataService } from "../services";
import { IDataField, IApiResult, Guid } from "../models";
import { Spin, Button } from "antd";
import { Toolbar } from "./toolbar";

interface IProps {
	entityTypeCode: string;
	uid?: Guid;
	onSuccess?: () => void;
}

interface IState {
	loading: boolean;
	typeData?: IDataField;
	data?: IDataField;
	typeFields?: IDataField[];
	commonFields?: IDataField[];
}

export class PaneEditMetadata extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	fetchData = async () => {
		const { entityTypeCode, uid } = this.props;

		const { type, ...values } = (uid) ? await this._metadataService.get(entityTypeCode, uid) : { type: "string" };

		const dataView = await this._metadataService.load("Metadata/Edit");

		this.setState({
			loading: false,
			typeData: { type: type },
			data: values as IDataField,
			typeFields: dataView.fields.slice(0, 1),
			commonFields: dataView.fields.slice(1)
		});
	};

	handleTypeChange = async (values: IDataField) => {
		console.log(values);
		this.setState({ typeData: { type: values.type } });
	};

	handleSubmit = async (values: IDataField): Promise<IApiResult> => {
		const { entityTypeCode, uid, onSuccess } = this.props,
			{ typeData } = this.state;

		const item = { type: typeData.type, ...values };

		let result;

		if (uid) {
			result = await this._metadataService.update(entityTypeCode, { uid, ...item });
		}
		else {
			result = await this._metadataService.insert({ entityTypeCode, item });
		}

		if (result.success && onSuccess) {
			onSuccess();
		}

		return result;
	};

	render = () => {
		const { loading, typeFields, commonFields, typeData, data } = this.state;

		return (
			<Spin spinning={loading}>

				<DataForm
					showControls={false}
					fields={typeFields}
					data={typeData}
					onChange={this.handleTypeChange} />

				<DataForm
					fields={commonFields}
					data={data}
					onSubmit={this.handleSubmit} />

				<Toolbar clear size="small">
					<Button size="small" icon="setting" />
				</Toolbar>

			</Spin>
		);
	};
}
