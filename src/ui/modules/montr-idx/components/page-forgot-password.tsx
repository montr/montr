import { DataForm, Icon, Page, useLocalStorage } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Spin } from "antd";
import * as React from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { LoginModel } from "../models";
import { Locale, Patterns, StorageNames, Views } from "../module";
import { AccountService } from "../services/account-service";

interface State {
	loading: boolean;
	data?: LoginModel;
	fields?: IDataField[];
}

export default function ForgotPassword() {

	const metadataService = new MetadataService(),
		accountService = new AccountService();

	const { t } = useTranslation(Locale.Namespace),
		[state, setState] = React.useState<State>({ loading: true }),
		[email, setEmail] = useLocalStorage(StorageNames.email, "");

	React.useEffect(() => {
		async function fetchData() {
			const dataView = await metadataService.load(Views.formForgotPassword);

			setState({ loading: false, fields: dataView.fields });
		}

		fetchData();

		return () => {
			metadataService.abort();
			accountService.abort();
		};
	}, []);

	async function handleChange(changedValues: LoginModel, values: LoginModel) {
		setEmail(values.email);
	}

	async function handleSubmit(values: LoginModel): Promise<ApiResult> {
		return await accountService.forgotPassword(values);
	}

	const { loading, fields } = state;

	return (
		<Page title={t("page.forgotPassword.title") as string}>

			<p>{t("page.forgotPassword.subtitle") as string}</p>

			<Spin spinning={loading}>
				<DataForm
					fields={fields}
					data={{ email }}
					onValuesChange={handleChange}
					onSubmit={handleSubmit}
					submitButton={t("button.forgotPassword")}
				/>
			</Spin>

			<p><Link to={Patterns.login}>{Icon.ArrowLeft} {t("page.register.link.login") as string}</Link></p>

		</Page>
	);
}
