import * as React from "react";
import { Page, DataForm } from "@montr-core/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Spin, Icon } from "antd";
import { MetadataService } from "@montr-core/services";
import { ILoginModel } from "../models/";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { Views } from "../module";
import { Link } from "react-router-dom";

interface IProps {
}

interface IState {
	loading: boolean;
	data: ILoginModel;
	fields?: IFormField[];
}

export default class ForgotPassword extends React.Component<IProps, IState> {

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
		const dataView = await this._metadataService.load(Views.formForgotPassword);

		this.setState({ loading: false, fields: dataView.fields });
	};

	send = async (values: ILoginModel): Promise<IApiResult> => {
		return await this._accountService.forgotPassword(values);
	};

	render = () => {
		const { fields, data, loading } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.forgotPassword.title")}>

					<p>{t("page.forgotPassword.subtitle")}</p>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={data}
							onSubmit={this.send}
							submitButton={t("button.forgotPassword")}
						/>
					</Spin>

					<p><Link to="/account/login"><Icon type="arrow-left" /> {t("page.register.link.login")}</Link></p>

				</Page>}
			</Translation>
		);
	};
}
