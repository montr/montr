import * as React from "react";
import { Page, DataForm, WrappedDataForm } from "@montr-core/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Spin } from "antd";
import { MetadataService, NavigationService } from "@montr-core/services";
import { ILoginModel } from "../models/";
import { Link } from "react-router-dom";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { Views } from "@montr-idx/module";
import { ExternalLoginForm } from "../components";

interface IProps {
}

interface IState {
	loading: boolean;
	data: ILoginModel;
	fields?: IFormField[];
}

export default class Login extends React.Component<IProps, IState> {

	private _navigation = new NavigationService();
	private _metadataService = new MetadataService();
	private _accountService = new AccountService();

	private _formRef: WrappedDataForm;

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

	saveFormRef = (formRef: WrappedDataForm) => {
		this._formRef = formRef;
	};

	fetchData = async () => {
		const dataView = await this._metadataService.load(Views.formLogin);

		this.setState({ loading: false, fields: dataView.fields });
	};

	login = async (values: ILoginModel): Promise<IApiResult> => {
		return await this._accountService.login({
			returnUrl: this._navigation.getReturnUrlParameter(),
			...values
		});
	};

	sendEmailConfirmation = async (): Promise<IApiResult> => {
		return await this._accountService.sendEmailConfirmation({
			email: await this._formRef.getFieldValue("email")
		});
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.login.title")}>

					<h3>{t("page.login.section.loginLocal")}</h3>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={data}
							wrappedComponentRef={this.saveFormRef}
							onSubmit={this.login}
							submitButton={t("button.login")}
							successMessage="User logged in."
						/>
					</Spin>

					<p><Link to="/account/forgot-password">{t("page.login.link.forgotPassword")}</Link></p>
					<p><Link to="/account/register">{t("page.login.link.register")}</Link></p>
					<p><a onClick={this.sendEmailConfirmation}>{t("page.login.link.resendEmailConfirmation")}</a></p>

					<h3>{t("page.login.section.loginExternal")}</h3>

					<ExternalLoginForm />

				</Page>}
			</Translation>
		);
	};
}
