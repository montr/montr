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
	typeFieldMap: { [key: string]: IDataField[]; };
	typeData?: IDataField;
	data?: IDataField;
	typeFields?: IDataField[];
	commonFields?: IDataField[];
	optionalFields?: IDataField[];
	visibleFields?: IDataField[];
}

// todo: read from server
const DefaultFieldType = "text";

export class PaneEditMetadata extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			typeFieldMap: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
	};

	fetchData = async () => {
		const { entityTypeCode, uid } = this.props;

		const { type, ...values } = (uid) ? await this._metadataService.get(entityTypeCode, uid) : { type: DefaultFieldType };

		const commonView = await this._metadataService.load("Metadata/Edit");

		const typeFields = commonView.fields.slice(0, 1),
			commonFields = commonView.fields.slice(1);

		const optionalFields = this.getOptionalFields(commonFields, values as IDataField);

		this.setState({
			loading: false,
			typeData: { type: type },
			data: values as IDataField,
			typeFields,
			commonFields,
			optionalFields
		}, () => this.setVisibleFields());
	};

	getOptionalFields = (fields: IDataField[], data: IDataField): IDataField[] => {
		return fields
			// todo: read optional field types from server
			.filter(x => !x.required && (x.type == "text" || x.type == "textarea"))
			.map(x => {
				// in optional fields using active as flag of visible field
				return { type: x.type, key: x.key, name: x.name, active: !!data[x.key] };
			});
	};

	setVisibleFields = async () => {
		const { typeData, typeFieldMap, commonFields, optionalFields } = this.state;

		let specificFields = typeFieldMap[typeData.type];

		if (!specificFields) {
			const typeView = await this._metadataService.load("Metadata/Edit/" + typeData.type);

			specificFields = typeFieldMap[typeData.type] = typeView.fields || [];
		}

		const visibleCommonFields = commonFields?.filter(field => {
			const optional = optionalFields.find(optional => field.key == optional.key);
			return !optional || optional.active;
		}) || [];

		const visibleFields = [...visibleCommonFields, ...specificFields];

		this.setState({ typeFieldMap, visibleFields });
	};

	handleTypeChange = async (values: IDataField) => {
		// todo: save/restore data between switching fields
		this.setState({ typeData: { type: values.type } }, () => this.setVisibleFields());
	};

	handleCheckOptionalField = (key: string, checked: boolean) => {
		const { optionalFields } = this.state;

		const field = optionalFields?.find(x => x.key == key);

		if (field) {
			field.active = checked;

			this.setState({ optionalFields }, () => this.setVisibleFields());
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
		const { loading, typeFields, visibleFields, optionalFields, typeData, data } = this.state;

		return (
			<Spin spinning={loading}>

				<DataForm
					showControls={false}
					fields={typeFields}
					data={typeData}
					onChange={this.handleTypeChange} />

				<DataForm
					fields={visibleFields}
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
