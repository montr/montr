import * as React from "react";
import { Page, DataForm } from "@montr-core/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Spin, Divider } from "antd";
import { MetadataService, NavigationService } from "@montr-core/services";
import { ILoginModel } from "../models/";
import { Link } from "react-router-dom";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { ExternalLoginForm } from "../components";
import { Views, Patterns } from "../module";

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
		const dataView = await this._metadataService.load(Views.formLogin);

		this.setState({ loading: false, fields: dataView.fields });
	};

	handleSubmit = async (values: ILoginModel): Promise<IApiResult> => {
		return await this._accountService.login({
			returnUrl: this._navigation.getReturnUrlParameter(),
			...values
		});
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.login.title")}>

					<p>{t("page.login.section.loginLocal")}</p>

					<Spin spinning={loading}>
						<DataForm
							layout="vertical"
							fields={fields}
							data={data}
							onSubmit={this.handleSubmit}
							submitButton={t("button.login")}
							successMessage={t("page.login.successMessage")}
						/>
					</Spin>

					<p>
						<Link to={Patterns.forgotPassword}>{t("page.login.link.forgotPassword")}</Link>
						<Divider type="vertical" />
						<Link to={Patterns.register}>{t("page.login.link.register")}</Link>
						<Divider type="vertical" />
						<Link to={Patterns.sendEmailConfirmation}>{t("page.login.link.resendEmailConfirmation")}</Link>
					</p>

					<Divider />

					<p>{t("page.login.section.loginExternal")}</p>

					<ExternalLoginForm />

				</Page>}
			</Translation>
		);
	};
}
