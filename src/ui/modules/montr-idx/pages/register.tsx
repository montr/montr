import * as React from "react";
import { Page, DataForm } from "@montr-core/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Spin } from "antd";
import { MetadataService } from "@montr-core/services";
import { IRegisterModel } from "../models/";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { ExternalLoginForm } from "../components";
import { Views } from "../module";
import { Link } from "react-router-dom";

interface IProps {
}

interface IState {
	loading: boolean;
	data: IRegisterModel;
	fields?: IFormField[];
}

export default class Register extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _accountService = new AccountService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			data: {},
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._accountService.abort();
	};

	fetchData = async () => {
		const dataView = await this._metadataService.load(Views.formRegister);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: IRegisterModel): Promise<IApiResult> => {
		return await this._accountService.register(values);
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.register.title")}>

					<h3>{t("page.register.subtitle")}</h3>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={data}
							onSubmit={this.save}
							submitButton={t("button.register")}
						/>
					</Spin>

					<p><Link to="/account/login">{t("page.register.link.login")}</Link></p>

					<h3>Use another service to register.</h3>

					<ExternalLoginForm />

				</Page>}
			</Translation>
		);
	};
}
