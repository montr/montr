import * as React from "react";
import { Page, DataForm, WrappedDataForm } from "@montr-core/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Row, Spin, Button, Col } from "antd";
import { MetadataService } from "@montr-core/services";
import { ILoginModel, IAuthScheme } from "../models/";
import { Link } from "react-router-dom";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { Constants } from "@montr-core/.";

interface IProps {
}

interface IState {
	loading: boolean;
	authSchemes: IAuthScheme[];
	data: ILoginModel;
	fields?: IFormField[];
}

export default class Login extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _accountService = new AccountService();

	private _formRef: WrappedDataForm;

	constructor(props: IProps) {
		super(props);

		this.state = {
			authSchemes: [],
			data: {},
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._accountService.abort();
	}

	saveFormRef = (formRef: WrappedDataForm) => {
		this._formRef = formRef;
	}

	fetchData = async () => {
		const dataView = await this._metadataService.load("Login/Form");

		const authSchemes = await this._accountService.authSchemes();

		this.setState({ loading: false, fields: dataView.fields, authSchemes });
	}

	login = async (values: ILoginModel): Promise<IApiResult> => {
		const result = await this._accountService.login(values);

		if (result.success) {
			window.location.href = this.getReturnUrl();
		}

		return result;
	}

	getReturnUrl = () => {
		// todo: check url is local
		const params = new URLSearchParams(window.location.search);
		return params.get("ReturnUrl") || "/";
	}

	sendEmailConfirmation = async (): Promise<IApiResult> => {
		return await this._accountService.sendEmailConfirmation({
			email: await this._formRef.getFieldValue("email")
		});
	}

	render = () => {
		const { fields, authSchemes, data, loading } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.login.title")}>
					<Row>
						<Col offset={4} span={8} style={{ padding: 12 }}>

							<h3>{t("page.login.section.loginLocal")}</h3>

							<div>
								<Spin spinning={loading}>
									<DataForm
										fields={fields}
										data={data}
										wrappedComponentRef={this.saveFormRef}
										onSubmit={this.login}
										submitButton={t("button.login")}
									/>
								</Spin>
							</div>

							<p><Link to="/account/forgot-password">{t("page.login.link.forgotPassword")}</Link></p>
							<p><Link to="/account/register">{t("page.login.link.register")}</Link></p>
							<p><a onClick={this.sendEmailConfirmation}>{t("page.login.link.resendEmailConfirmation")}</a></p>

						</Col>
						<Col span={8} style={{ padding: 12 }}>

							<h3>{t("page.login.section.loginExternal")}</h3>

							<p>
								<form method="post" action={`${Constants.apiURL}/authentication/externalLogin`}>
									<input type="hidden" name="returnUrl" value={this.getReturnUrl()} />
									{authSchemes.map(x => (
										<Button
											key={x.name}
											htmlType="submit"
											name="provider"
											value={x.name}
											icon={x.name.toLowerCase()}>
											{`Log in using your ${x.displayName} account`}
										</Button>
									))}
								</form>
							</p>

						</Col>
					</Row>
				</Page>}
			</Translation>
		);
	}
}
