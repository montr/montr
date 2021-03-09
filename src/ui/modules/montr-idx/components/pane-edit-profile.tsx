import React from "react";
import { IDataField, ApiResult } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { ProfileModel } from "../models";
import { ProfileService } from "../services";
import { Locale, Views } from "../module";
import { PageHeader, DataForm } from "@montr-core/components";
import { Translation } from "react-i18next";
import { Spin } from "antd";

interface Props {
}

interface State {
	loading: boolean;
	data: ProfileModel;
	fields?: IDataField[];
}

export default class PaneEditProfile extends React.Component<Props, State> {

	private _metadataService = new MetadataService();
	private _profileService = new ProfileService();

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

		const dataView = await this._metadataService.load(Views.formUpdateProfile);

		this.setState({ loading: false, data, fields: dataView.fields });
	};

	save = async (values: ProfileModel): Promise<ApiResult> => {
		return await this._profileService.update(values);
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <>

					<PageHeader>{t("page.profile.title")}</PageHeader>
					<h3>{t("page.profile.subtitle")}</h3>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={data}
							onSubmit={this.save}
							successMessage="Your profile has been updated"
						/>
					</Spin>
				</>}
			</Translation>
		);
	};
}
