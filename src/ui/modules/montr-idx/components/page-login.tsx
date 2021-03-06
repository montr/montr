import * as React from "react";
import { Link } from "react-router-dom";
import { Spin, Divider } from "antd";
import { useTranslation } from "react-i18next";
import { useLocalStorage, Page, DataForm } from "@montr-core/components";
import { IDataField, ApiResult } from "@montr-core/models";
import { MetadataService, NavigationService } from "@montr-core/services";
import { LoginModel } from "../models";
import { AccountService } from "../services/account-service";
import { ExternalLoginForm } from ".";
import { Views, Patterns, StorageNames, Locale } from "../module";

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default function Login() {

	const navigation = new NavigationService(),
		metadataService = new MetadataService(),
		accountService = new AccountService();

	const { t } = useTranslation(Locale.Namespace),
		[state, setState] = React.useState<State>({ loading: true }),
		[email, setEmail] = useLocalStorage(StorageNames.email, "");

	React.useEffect(() => {
		async function fetchData() {
			const dataView = await metadataService.load(Views.formLogin);

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
		return await accountService.login({
			returnUrl: navigation.getReturnUrlParameter() ?? "/",
			...values
		});
	};

	const { loading, fields } = state;

	const data: LoginModel = { email, rememberMe: true };

	return (
		<Page title={t("page.login.title")}>

			<p>{t("page.login.section.loginLocal")}</p>

			<Spin spinning={loading}>
				<DataForm
					layout="vertical"
					hideLabels
					fields={fields}
					data={data}
					onChange={handleChange}
					onSubmit={handleSubmit}
					submitButton={t("button.login")}
					successMessage={t("page.login.successMessage")}
				/>
			</Spin>

			<Link to={Patterns.forgotPassword}>{t("page.login.link.forgotPassword")}</Link>
			<Divider type="vertical" />
			<Link to={Patterns.register}>{t("page.login.link.register")}</Link>
			<Divider type="vertical" />
			<Link to={Patterns.sendEmailConfirmation}>{t("page.login.link.resendEmailConfirmation")}</Link>

			<Divider />

			<p>{t("page.login.section.loginExternal")}</p>

			<ExternalLoginForm />

		</Page>
	);
}
