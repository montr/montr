import { DataForm } from "@montr-core/components/data-form";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services/metadata-service";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/es/form";
import React from "react";
import { Translation } from "react-i18next";
import { ProfileModel } from "../models/profile-model";
import { Locale, Views } from "../module";
import { ProfileService } from "../services/profile-service";

interface Props {
	onSuccess?: () => void;
	onCancel?: () => void;
}

interface State {
	loading: boolean;
	data: ProfileModel;
	fields?: IDataField[];
}

/* todo: extract modal with form component to prevent copying _formRef code */
export class ModalChangeEmail extends React.Component<Props, State> {

	private readonly metadataService = new MetadataService();
	private readonly profileService = new ProfileService();

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
		await this.metadataService.abort();
		await this.profileService.abort();
	};

	fetchData = async () => {
		const data = await this.profileService.get();

		const dataView = await this.metadataService.load(Views.formChangeEmail);

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

		const result = await this.profileService.changeEmail(values);

		if (result.success) {
			if (onSuccess) await onSuccess();
		}

		return result;
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Modal open={!loading} title={t("page.changeEmail.title") as string}
					onOk={this.onOk} onCancel={this.onCancel}
					okText={t("button.updateEmail") as string} /* width="640px" */>

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
