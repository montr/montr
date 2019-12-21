import React from "react";
import { Modal, Spin } from "antd";
import { Translation } from "react-i18next";
import { IDataField, IApiResult } from "@montr-core/models";
import { DataForm } from "@montr-core/components";
import { MetadataService } from "@montr-core/services";
import { ProfileService } from "../services";
import { IProfileModel } from "../models";
import { Views } from "../module";
import { FormInstance } from "antd/lib/form";

interface IProps {
	onSuccess?: () => void;
	onCancel?: () => void;
}

interface IState {
	loading: boolean;
	data: IProfileModel;
	fields?: IDataField[];
}

/* todo: extract modal with form component to prevent copying _formRef code */
export class ModalChangeEmail extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _profileService = new ProfileService();

	private _formRef = React.createRef<FormInstance>();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._profileService.abort();
	};

	fetchData = async () => {
		const data = await this._profileService.get();

		const dataView = await this._metadataService.load(Views.formChangeEmail);

		this.setState({ loading: false, data, fields: dataView.fields });
	};

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.current.submit();
	};

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	};

	handleSubmit = async (values: IProfileModel): Promise<IApiResult> => {
		const { onSuccess } = this.props;

		const result = await this._profileService.changeEmail(values);

		if (result.success) {
			if (onSuccess) await onSuccess();
		}

		return result;
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Modal visible={!loading} title={t("page.changeEmail.title")}
					onOk={this.onOk} onCancel={this.onCancel}
					okText={t("button.updateEmail")} /* width="640px" */>

					<Spin spinning={loading}>
						<DataForm
							formRef={this._formRef}
							fields={fields}
							data={data}
							showControls={false}
							onSubmit={this.handleSubmit}
						/>
					</Spin>
				</Modal>}
			</Translation>
		);
	};
}
