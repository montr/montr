import React from "react";
import { IFormField, IApiResult } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Views } from "../module";
import { IProfileModel } from "../models";
import { ProfileService } from "../services/";
import { Translation } from "react-i18next";
import { Page, DataForm } from "@montr-core/components";
import { Spin } from "antd";

interface IProps {
}

interface IState {
	loading: boolean;
	data: IProfileModel;
	fields?: IFormField[];
}

export default class Register extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _profileService = new ProfileService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			data: {},
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._profileService.abort();
	}

	fetchData = async () => {
		const dataView = await this._metadataService.load(Views.formProfile);

		this.setState({ loading: false, fields: dataView.fields });
	}

	save = async (values: IProfileModel): Promise<IApiResult> => {
		return await this._profileService.update(values);
	}

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.profile.title")}>
					<h3>{t("page.profile.subtitle")}</h3>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={data}
							onSubmit={this.save}
						/>
					</Spin>
				</Page>}
			</Translation>
		);
	}
}
