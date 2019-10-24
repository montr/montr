import * as React from "react";
import { Page, DataForm } from "../components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Spin } from "antd";
import { MetadataService } from "@montr-core/services";
import { ILoginModel } from "@montr-core/models/login-model";
import { Translation } from "react-i18next";

interface IProps {
}

interface IState {
	loading: boolean;
	data: ILoginModel;
	fields?: IFormField[];
}

export default class Register extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();

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
	}

	fetchData = async () => {
		const dataView = await this._metadataService.load("Register/Form");

		this.setState({ loading: false, fields: dataView.fields });
	}

	save = async (values: ILoginModel): Promise<IApiResult> => {
		return null;
	}

	render = () => {
		const { fields, data, loading } = this.state;

		return (
			<Translation>
				{(t, { i18n }) => (
					<Page title="Register">

						<h3>Create a new account.</h3>

						<div style={{ width: "50%" }} >
							<Spin spinning={loading}>
								<DataForm
									fields={fields}
									data={data}
									onSubmit={this.save}
									submitButton={t("button.register")}
								/>
							</Spin>
						</div>

					</Page>
				)}
			</Translation>
		);
	}
}
