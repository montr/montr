import { DataForm, Page } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Button, Spin } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { RouteComponentProps, useNavigate } from "react-router";
import { ResetPasswordModel } from "../models";
import { Locale, Patterns, Views } from "../module";
import { AccountService } from "../services/account-service";

interface RouteProps {
	code: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default class ResetPassword extends React.Component<Props, State> {

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
		const dataView = await this._metadataService.load(Views.formResetPassword);

		this.setState({ loading: false, fields: dataView.fields });
	};

	resetPassword = async (values: ResetPasswordModel): Promise<ApiResult> => {
		const { code } = this.props.match.params;

		return await this._accountService.resetPassword({ code, ...values });
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
