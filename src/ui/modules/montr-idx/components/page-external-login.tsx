import * as React from "react";
import { Page, DataForm, Icon } from "@montr-core/components";
import { Spin } from "antd";
import { Translation, Trans } from "react-i18next";
import { AccountService } from "../services/account-service";
import { ExternalRegisterModel } from "../models";
import { MetadataService, NavigationService, OperationService } from "@montr-core/services";
import { RouteComponentProps } from "react-router";
import { Locale, Patterns, Views } from "../module";
import { IDataField, ApiResult } from "@montr-core/models";
import { Constants } from "@montr-core/constants";
import { Link } from "react-router-dom";

interface Props extends RouteComponentProps {
}

interface State {
	loading: boolean;
	data?: ExternalRegisterModel;
	fields?: IDataField[];
}

export default class ExternalLogin extends React.Component<Props, State> {

	private _operation = new OperationService();
	private _navigation = new NavigationService();
	private _metadataService = new MetadataService();
	private _accountService = new AccountService();

	constructor(props: Props) {
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
				this.props.history.push(Patterns.login);
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
