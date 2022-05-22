import { DataForm, Page } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Button, Spin } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import { ResetPasswordModel } from "../models";
import { Locale, Patterns, Views } from "../module";
import { AccountService } from "../services/account-service";

interface RouteProps {
	code?: string;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default class ResetPassword extends React.Component<unknown, State> {

	private readonly metadataService = new MetadataService();
	private readonly accountService = new AccountService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true
		};
	}

	getRouteProps = (): RouteProps => {
		return useParams();
	};

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this.metadataService.abort();
		await this.accountService.abort();
	};

	fetchData = async () => {
		const dataView = await this.metadataService.load(Views.formResetPassword);

		this.setState({ loading: false, fields: dataView.fields });
	};

	resetPassword = async (values: ResetPasswordModel): Promise<ApiResult> => {
		const { code } = this.getRouteProps();

		return await this.accountService.resetPassword({ code, ...values });
	};

	handleContinue = async () => {
		const navigate = useNavigate();

		navigate(Patterns.login);
	};

	render = () => {
		const { fields, loading } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.resetPassword.title")}>

					<p>{t("page.resetPassword.subtitle")}</p>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={{}}
							onSubmit={this.resetPassword}
							submitButton={t("button.resetPassword")}
						/>

						{!loading && <p>Your password has been reset.</p>}

						<Button disabled={loading} onClick={this.handleContinue}>{t("button.continue")}</Button>
					</Spin>
				</Page>}
			</Translation>
		);
	};
}
