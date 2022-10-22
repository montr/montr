import { DataForm } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import React from "react";
import { Translation } from "react-i18next";
import { ProfileModel } from "../models";
import { Locale, Views } from "../module";
import { ProfileService } from "../services";

interface Props {
	onSuccess?: () => void;
	onCancel?: () => void;
}

interface State {
	loading: boolean;
	data: ProfileModel;
	fields?: IDataField[];
}

export class ModalChangePhone extends React.Component<Props, State> {

	private _metadataService = new MetadataService();
	private _profileService = new ProfileService();

	private _formRef = React.createRef<FormInstance>();

	constructor(props: Props) {
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

		const dataView = await this._metadataService.load(Views.formChangePhone);

		this.setState({ loading: false, data, fields: dataView.fields });
	};

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.current.submit();
	};

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	};

	handleSubmit = async (values: ProfileModel): Promise<ApiResult> => {
		const { onSuccess } = this.props;

		const result = await this._profileService.changePhone(values);

		if (result.success) {
			if (onSuccess) await onSuccess();
		}

		return result;
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Modal visible={!loading} title={t("page.changePhone.title") as string}
					onOk={this.onOk} onCancel={this.onCancel}
					okText={t("button.updatePhone") as string} /* width="640px" */>

					<Spin spinning={loading}>
						<DataForm
							formRef={this._formRef}
							fields={fields}
							data={data}
							hideButtons={true}
							onSubmit={this.handleSubmit}
						/>
					</Spin>
				</Modal>}
			</Translation>
		);
	};
}
