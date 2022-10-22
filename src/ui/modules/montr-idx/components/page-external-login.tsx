import { DataForm, Icon, Page } from "@montr-core/components";
import { Constants } from "@montr-core/constants";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService, NavigationService, OperationService } from "@montr-core/services";
import { Spin } from "antd";
import * as React from "react";
import { Trans, Translation } from "react-i18next";
import { Link, Navigate } from "react-router-dom";
import { ExternalRegisterModel } from "../models";
import { Locale, Patterns, Views } from "../module";
import { AccountService } from "../services/account-service";

interface State {
	loading: boolean;
	navigateTo?: string;
	data?: ExternalRegisterModel;
	fields?: IDataField[];
}

export default class ExternalLogin extends React.Component<unknown, State> {

	private readonly operation = new OperationService();
	private readonly navigation = new NavigationService();
	private readonly metadataService = new MetadataService();
	private readonly accountService = new AccountService();

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
		await this.metadataService.abort();
		await this.accountService.abort();
	};

	fetchData = async () => {
		await this.operation.execute(async () => {
			const result = await this.accountService.externalLoginCallback({
				returnUrl: this.navigation.getUrlParameter(Constants.returnUrlParamLower),
				remoteError: this.navigation.getUrlParameter("remoteError")
			});

			// this.setState({ loading: false });

			if (result.success && result.data) {
				const dataView = await this.metadataService.load(Views.formExternalRegister);

				this.setState({ loading: false, data: result.data, fields: dataView.fields });
			}
			else {
				this.setState({ navigateTo: Patterns.login });
			}

			return result;
		});
	};

	handleSubmit = async (values: ExternalRegisterModel): Promise<ApiResult> => {
		const { data } = this.state;

		return await this.accountService.externalRegister({
			returnUrl: data.returnUrl,
			...values
		});
	};

	render = () => {
		const { loading, navigateTo, fields, data } = this.state;

		if (navigateTo) {
			return <Navigate to={navigateTo} />;
		}

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.externalLogin.title") as string}>
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

							<p><Link to={Patterns.login}>{Icon.ArrowLeft} {t("page.register.link.login") as string}</Link></p>

						</>}

					</Spin>
				</Page>}
			</Translation>
		);
	};
}
