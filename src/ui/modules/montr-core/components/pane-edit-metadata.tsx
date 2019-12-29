import React from "react";
import { DataForm } from ".";
import { MetadataService } from "../services";
import { IDataField, IApiResult, Guid } from "../models";
import { Spin, Button, Popover, Switch, List } from "antd";
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
	optionalFields?: IDataField[];
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

		const typeFields = dataView.fields.slice(0, 1),
			commonFields = dataView.fields.slice(1);

		const optionalFields = this.getOptionalFields(commonFields, values as IDataField);

		this.setState({
			loading: false,
			typeData: { type: type },
			data: values as IDataField,
			typeFields,
			commonFields,
			optionalFields
		});
	};

	getOptionalFields = (fields: IDataField[], data: IDataField): IDataField[] => {
		return fields
			.filter(x => !x.required && (x.type == "string" || x.type == "textarea"))
			.map(x => {
				// in optional fields using active as flag of visible field
				return { type: x.type, key: x.key, name: x.name, active: !!data[x.key] };
			});
	};

	handleTypeChange = async (values: IDataField) => {
		this.setState({ typeData: { type: values.type } });
	};

	handleCheckOptionalField = (key: string, checked: boolean) => {
		const { optionalFields } = this.state;

		const field = optionalFields?.find(x => x.key == key);

		if (field) {
			field.active = checked;

			this.setState({ optionalFields });
		}
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

	renderPopover = (optionalFields: IDataField[]) => {
		return (
			<List size="small" bordered={false}>
				{optionalFields && optionalFields.map(x => {
					return (
						<List.Item key={x.key} style={{ border: 0, padding: 2 }}
							actions={[
								<Switch size="small" checked={x.active} onChange={(checked) => {
									this.handleCheckOptionalField(x.key, checked);
								}} />
							]} >
							<div style={{ width: "100%" }}>{x.name}</div>
						</List.Item>
					);
				})}
			</List>
		);
	};

	render = () => {
		const { loading, typeFields, commonFields, optionalFields, typeData, data } = this.state;

		// todo: move to fetchData
		const visibleCommonFields = commonFields?.filter(field => {
			const optional = optionalFields.find(optional => field.key == optional.key);
			return !optional || optional.active;
		});

		return (
			<Spin spinning={loading}>

				<DataForm
					showControls={false}
					fields={typeFields}
					data={typeData}
					onChange={this.handleTypeChange} />

				<DataForm
					fields={visibleCommonFields}
					data={data}
					onSubmit={this.handleSubmit} />

				<Toolbar clear size="small">
					<Popover content={this.renderPopover(optionalFields)} trigger="click" placement="topLeft">
						<Button type="link" icon="setting" />
					</Popover>
				</Toolbar>

			</Spin>
		);
	};
}
