import { DataForm, Icon, Page } from "@montr-core/components";
import { Constants } from "@montr-core/constants";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService, NavigationService, OperationService } from "@montr-core/services";
import { Spin } from "antd";
import * as React from "react";
import { Trans, Translation } from "react-i18next";
import { Link, useNavigate } from "react-router-dom";
import { ExternalRegisterModel } from "../models";
import { Locale, Patterns, Views } from "../module";
import { AccountService } from "../services/account-service";

interface State {
	loading: boolean;
	data?: ExternalRegisterModel;
	fields?: IDataField[];
}

export default class ExternalLogin extends React.Component<unknown, State> {

	private _operation = new OperationService();
	private _navigation = new NavigationService();
	private _metadataService = new MetadataService();
	private _accountService = new AccountService();

	constructor(props: unknown) {
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
		await this._operation.execute(async () => {
			const result = await this._accountService.externalLoginCallback({
				returnUrl: this._navigation.getUrlParameter(Constants.returnUrlParamLower),
				remoteError: this._navigation.getUrlParameter("remoteError")
			});

			// this.setState({ loading: false });

			if (result.success && result.data) {
				const dataView = await this._metadataService.load(Views.formExternalRegister);

				this.setState({ loading: false, data: result.data, fields: dataView.fields });
			}
			else {
				const navigate = useNavigate();

				navigate(Patterns.login);
			}

			return result;
		});
	};

	handleSubmit = async (values: ExternalRegisterModel): Promise<ApiResult> => {
		const { data } = this.state;

		return await this._accountService.externalRegister({
			returnUrl: data.returnUrl,
			...values
		});
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.externalLogin.title")}>
					<Spin spinning={loading}>

						{data && <>
							<p>
								<Trans ns={Locale.Namespace} i18nKey="page.externalLogin.subtitle" values={{ provider: data.provider }}>
									<strong>{data.provider}</strong>
								</Trans>
							</p>

							<DataForm
								fields={fields}
								data={data}
								onSubmit={this.handleSubmit}
								submitButton={t("button.register")}
							/>

							<p><Link to={Patterns.login}>{Icon.ArrowLeft} {t("page.register.link.login")}</Link></p>

						</>}

					</Spin>
				</Page>}
			</Translation>
		);
	};
}
