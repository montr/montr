import * as React from "react";
import { Page, DataForm } from "@montr-core/components";
import { Spin } from "antd";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { IExternalRegisterModel } from "../models";
import { MetadataService, NavigationService, OperationService } from "@montr-core/services";
import { RouteComponentProps } from "react-router";
import { Patterns, Views } from "@montr-idx/module";
import { IFormField, IApiResult } from "@montr-core/models";
import { Constants } from "@montr-core/constants";

interface IProps extends RouteComponentProps {
}

interface IState {
	loading: boolean;
	data?: IExternalRegisterModel;
	fields?: IFormField[];
}

export default class ExternalLogin extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _navigation = new NavigationService();
	private _metadataService = new MetadataService();
	private _accountService = new AccountService();

	constructor(props: IProps) {
		super(props);

		this.state = {
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

		const result = await this._operation.execute(() => this._accountService.externalLoginCallback({
			returnUrl: this._navigation.getUrlParameter(Constants.returnUrlParamLower),
			remoteError: this._navigation.getUrlParameter("remoteError")
		}));

		// this.setState({ loading: false });

		if (result.success && result.data) {
			const dataView = await this._metadataService.load(Views.formExternalRegister);

			this.setState({ loading: false, data: result.data, fields: dataView.fields });
		}
		else {
			this.props.history.push(Patterns.login);
		}
	};

	handleSubmit = async (values: IExternalRegisterModel): Promise<IApiResult> => {
		const { data } = this.state;

		return await this._accountService.externalRegister({
			returnUrl: data.returnUrl,
			...values
		});
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.externalLogin.title")}>
					<Spin spinning={loading}>

						{data && <>
							<p>
								You've successfully authenticated with <strong>{data.provider}</strong>.
								Please enter an email address for this site below and click the Register button to finish
								logging in.
							</p>

							<DataForm
								fields={fields}
								data={data}
								onSubmit={this.handleSubmit}
								submitButton={t("button.register")}
							/>
						</>}

					</Spin>
				</Page>}
			</Translation>
		);
	};
}
