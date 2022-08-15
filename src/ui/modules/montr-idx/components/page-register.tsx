import { DataForm, Icon, Page } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Divider, Spin } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { Link } from "react-router-dom";
import { ExternalLoginForm } from ".";
import { RegisterModel } from "../models";
import { Locale, Patterns, Views } from "../module";
import { AccountService } from "../services/account-service";

interface State {
	loading: boolean;
	data: RegisterModel;
	fields?: IDataField[];
}

export default class Register extends React.Component<unknown, State> {

	private readonly metadataService = new MetadataService();
	private readonly accountService = new AccountService();

	constructor(props: unknown) {
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
		await this.metadataService.abort();
		await this.accountService.abort();
	};

	fetchData = async () => {
		const dataView = await this.metadataService.load(Views.formRegister);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: RegisterModel): Promise<ApiResult> => {
		return await this.accountService.register(values);
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.register.title")}>

					<p>{t("page.register.subtitle")}</p>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={data}
							onSubmit={this.save}
							submitButton={t("button.register")}
						/>
					</Spin>

					{/* todo: move link to separate component */}
					<p><Link to={Patterns.login}>{Icon.ArrowLeft} {t("page.register.link.login")}</Link></p>

					<Divider />

					<p>{t("page.register.section.registerExternal")}</p>

					<ExternalLoginForm />

				</Page>}
			</Translation>
		);
	};
}
