import * as React from "react";
import { Page, DataForm } from "@montr-core/components";
import { Spin } from "antd";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { IExternalLoginModel, IExternalLoginResult, IExternalRegisterUser } from "../models";
import { NotificationService, MetadataService } from "@montr-core/services";
import { RouteComponentProps } from "react-router";
import { Patterns } from "@montr-idx/routes";
import { IFormField, IApiResult } from "@montr-core/models";

interface IProps extends RouteComponentProps {
}

interface IState {
	loading: boolean;
	data?: IExternalRegisterUser;
	fields?: IFormField[];
}

export default class ExternalLogin extends React.Component<IProps, IState> {

	private _notification = new NotificationService();
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
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._accountService.abort();
	}

	fetchData = async () => {

		const params = new URLSearchParams(window.location.search);

		const request: IExternalLoginModel = {
			returnUrl: params.get("returnUrl"),
			remoteError: params.get("remoteError")
		};

		const result: IExternalLoginResult = await this._accountService.externalLoginCallback(request);

		this.setState({ loading: false });

		if (result.success) {
			if (result.redirectUrl) {
				window.location.href = result.redirectUrl;
			}

			if (result.register) {

				const dataView = await this._metadataService.load("ExternalRegister/Form");

				this.setState({ loading: false, data: result.register, fields: dataView.fields });
			}
		}
		else {
			if (result.errors) {
				this._notification.error(result.errors[0].messages);
			}

			this.props.history.push(Patterns.login);
		}
	}

	save = async (values: IExternalRegisterUser): Promise<IApiResult> => {
		const { data } = this.state;

		const result = await this._accountService.externalRegister({
			returnUrl: data.returnUrl,
			...values
		});

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

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.externalLogin.title")}>
					<Spin spinning={loading}>

						{data && <>
							<div style={{ width: "50%" }} >
								<p>
									You've successfully authenticated with <strong>{data.provider}</strong>.
									Please enter an email address for this site below and click the Register button to finish
									logging in.
							</p>

								<DataForm
									fields={fields}
									data={data}
									onSubmit={this.save}
									submitButton={t("button.register")}
								/>
							</div>
						</>}

					</Spin>
				</Page>}
			</Translation>
		);
	}
}
