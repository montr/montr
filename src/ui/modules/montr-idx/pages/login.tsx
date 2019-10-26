import * as React from "react";
import { Page, DataForm } from "@montr-core/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Spin } from "antd";
import { MetadataService } from "@montr-core/services";
import { ILoginModel } from "../models/";
import { Link } from "react-router-dom";
import { Translation } from "react-i18next";

interface IProps {
}

interface IState {
	loading: boolean;
	data: ILoginModel;
	fields?: IFormField[];
}

export default class Login extends React.Component<IProps, IState> {

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
		const dataView = await this._metadataService.load("Login/Form");

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
					<Page title="Log in">

						<h3>Use a local account to log in.</h3>

						<div style={{ width: "50%" }} >
							<Spin spinning={loading}>
								<DataForm
									fields={fields}
									data={data}
									onSubmit={this.save}
									submitButton={t("button.login")}
								/>
							</Spin>
						</div>

						<p><Link to="/account/forgot-password">Forgot your password?</Link></p>
						<p><Link to="/account/register">Register as a new user</Link></p>

						<h3>Use another service to log in.</h3>

					</Page>
				)}
			</Translation>
		);
	}
}
