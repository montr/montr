import React from "react";
import { IDataField, ApiResult } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { ProfileModel } from "../models";
import { ProfileService } from "../services";
import { Locale, Views } from "../module";
import { PageHeader, DataForm } from "@montr-core/components";
import { Translation } from "react-i18next";
import { Spin } from "antd";

interface State {
	loading: boolean;
	data: ProfileModel;
	fields?: IDataField[];
}

export default class PaneEditProfile extends React.Component<null, State> {

	private readonly metadataService = new MetadataService();
	private readonly profileService = new ProfileService();

	constructor() {
		super(null);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.metadataService.abort();
		await this.profileService.abort();
	};

	fetchData = async (): Promise<void> => {
		const data = await this.profileService.get();

		const dataView = await this.metadataService.load(Views.formUpdateProfile);

		this.setState({ loading: false, data, fields: dataView.fields });
	};

	save = async (values: ProfileModel): Promise<ApiResult> => {
		return await this.profileService.update(values);
	};

	render = (): React.ReactNode => {
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
