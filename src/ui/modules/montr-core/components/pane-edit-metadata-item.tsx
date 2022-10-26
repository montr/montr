import { Button, Drawer, List, Popover, Spin, Switch } from "antd";
import { FormInstance } from "antd/lib/form";
import React from "react";
import { ButtonCancel, ButtonSave, DataForm, Icon, Toolbar } from ".";
import { ApiResult, Guid, IDataField } from "../models";
import { Views } from "../module";
import { DataHelper, MetadataService } from "../services";

interface Props {
	mode: "field" | "form";
	entityTypeCode: string;
	entityUid: Guid;
	uid?: Guid;
	onSuccess?: () => void;
	onClose?: () => void;
}

interface State {
	loading: boolean;
	typeFieldMap: { [key: string]: IDataField[]; };
	typeData?: IDataField;
	data?: IDataField;
	typeFields?: IDataField[];
	optionalFields?: IDataField[];
	visibleFields?: IDataField[];
}

// todo: read from server
const DefaultFieldType = "text";

export class PaneEditMetadataItem extends React.Component<Props, State> {

	private metadataService = new MetadataService();
	private formRef = React.createRef<FormInstance>();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
			typeFieldMap: {}
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.metadataService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { entityTypeCode, entityUid, uid } = this.props;

		const { type, ...values } = (uid)
			? await this.metadataService.get(entityTypeCode, entityUid, uid)
			: { type: DefaultFieldType };

		const dataView = await this.metadataService.load(Views.fieldmetadataForm);

		this.setState({
			loading: false,
			typeData: { type: type },
			data: values as IDataField,
			typeFields: dataView?.fields || [],
			optionalFields: []
		}, () => this.setVisibleFields(true));
	};

	rebuildOptionalFields = (fields: IDataField[], currentOptionalFields: IDataField[], data: IDataField): IDataField[] => {
		// todo: read optional field types from server
		return fields
			.filter(x => !x.required && (x.type == "text" || x.type == "textarea"))
			.map(x => {
				const current = currentOptionalFields.find(op => op.key == x.key);
				const value = DataHelper.indexer(data, x.key, undefined);

				// in optional fields using active as flag of visible field
				return { type: x.type, key: x.key, name: x.name, active: current?.active || !!value };
			});
	};

	setVisibleFields = async (rebuildOptionalFields: boolean): Promise<void> => {
		const { typeData, data, typeFieldMap, optionalFields: currentOptionalFields } = this.state;

		let specificFields = typeFieldMap[typeData.type];

		if (!specificFields) {
			const dataView = await this.metadataService.load("Metadata/Edit/" + typeData.type);

			specificFields = typeFieldMap[typeData.type] = dataView?.fields || [];
		}

		const optionalFields = (rebuildOptionalFields)
			? this.rebuildOptionalFields(specificFields, currentOptionalFields, data)
			: currentOptionalFields;

		const visibleFields = specificFields.filter(field => {
			const optional = optionalFields.find(optional => field.key == optional.key);
			return !optional || optional.active;
		}) || [];

		this.setState({ typeFieldMap, optionalFields, visibleFields });
	};

	handleTypeChange = async (changedValues: IDataField, values: IDataField): Promise<void> => {
		// todo: save/restore data between switching fields
		this.setState({ typeData: { type: values.type } }, () => this.setVisibleFields(true));
	};

	handleCheckOptionalField = (key: string, checked: boolean): void => {
		const { optionalFields } = this.state;

		const field = optionalFields?.find(x => x.key == key);

		if (field) {
			field.active = checked;

			this.setState({ optionalFields }, () => this.setVisibleFields(false));
		}
	};

	handleSubmitClick = async (e: React.MouseEvent<any>): Promise<void> => {
		await this.formRef.current.submit();
	};

	handleSubmit = async (values: IDataField): Promise<ApiResult> => {
		const { entityTypeCode, entityUid, uid, onSuccess } = this.props,
			{ typeData } = this.state;

		const item = { type: typeData.type, ...values };

		let result;

		if (uid) {
			result = await this.metadataService.update({ entityTypeCode, entityUid, item: { uid, ...item } });
		}
		else {
			result = await this.metadataService.insert({ entityTypeCode, entityUid, item });
		}

		if (result.success && onSuccess) {
			onSuccess();
		}

		return result;
	};

	renderPopover = (optionalFields: IDataField[]): React.ReactNode => {
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

	render = (): React.ReactNode => {
		const { onClose } = this.props,
			{ loading, typeFields, visibleFields, optionalFields, typeData, data } = this.state;

		return (<>
			<Spin spinning={loading}>
				<Drawer
					title="Metadata"
					closable={true}
					onClose={onClose}
					visible={true}
					width={720}
					footer={
						<Toolbar clear size="small" float="right">
							<Popover content={this.renderPopover(optionalFields)} trigger="click" placement="topLeft">
								<Button type="link" icon={Icon.Setting} />
							</Popover>
							<ButtonCancel onClick={onClose} />
							<ButtonSave onClick={this.handleSubmitClick} />
						</Toolbar>}>

					<DataForm
						hideButtons={true}
						fields={typeFields}
						data={typeData}
						onValuesChange={this.handleTypeChange} />

					<DataForm
						formRef={this.formRef}
						hideButtons={true}
						fields={visibleFields}
						data={data}
						onSubmit={this.handleSubmit} />

				</Drawer>
			</Spin>
		</>);
	};
}
