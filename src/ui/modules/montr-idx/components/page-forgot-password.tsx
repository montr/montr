import * as React from "react";
import { useLocalStorage, Page, DataForm, Icon } from "@montr-core/components";
import { IDataField, ApiResult } from "@montr-core/models";
import { Spin } from "antd";
import { MetadataService } from "@montr-core/services";
import { LoginModel } from "../models";
import { useTranslation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { Views, Patterns, StorageNames, Locale } from "../module";
import { Link } from "react-router-dom";

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

	async function handleChange(values: LoginModel) {
		setEmail(values.email);
	};

	async function handleSubmit(values: LoginModel): Promise<ApiResult> {
		return await accountService.forgotPassword(values);
	};

	const { loading, fields } = state;

	return (
		<Page title={t("page.forgotPassword.title")}>

			<p>{t("page.forgotPassword.subtitle")}</p>

			<Spin spinning={loading}>
				<DataForm
					fields={fields}
					data={{ email }}
					onChange={handleChange}
					onSubmit={handleSubmit}
					submitButton={t("button.forgotPassword")}
				/>
			</Spin>

			<p><Link to={Patterns.login}>{Icon.ArrowLeft} {t("page.register.link.login")}</Link></p>

		</Page>
	);
}
