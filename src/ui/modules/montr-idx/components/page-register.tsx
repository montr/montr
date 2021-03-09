import * as React from "react";
import { Page, DataForm, Icon } from "@montr-core/components";
import { IDataField, ApiResult } from "@montr-core/models";
import { Spin, Divider } from "antd";
import { MetadataService } from "@montr-core/services";
import { RegisterModel } from "../models";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { ExternalLoginForm } from ".";
import { Views, Patterns, Locale } from "../module";
import { Link } from "react-router-dom";

interface Props {
}

interface State {
	loading: boolean;
	data: RegisterModel;
	fields?: IDataField[];
}

export default class Register extends React.Component<Props, State> {

	private _metadataService = new MetadataService();
	private _accountService = new AccountService();

	constructor(props: Props) {
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
		const dataView = await this._metadataService.load(Views.formRegister);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: RegisterModel): Promise<ApiResult> => {
		return await this._accountService.register(values);
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
