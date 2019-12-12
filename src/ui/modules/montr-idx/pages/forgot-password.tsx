import * as React from "react";
import { useLocalStorage } from "@montr-core/hooks";
import { Page, DataForm } from "@montr-core/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { Spin, Icon } from "antd";
import { MetadataService } from "@montr-core/services";
import { ILoginModel } from "../models/";
import { useTranslation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { Views, Patterns, StorageNames } from "../module";
import { Link } from "react-router-dom";

interface IState {
	loading: boolean;
	data?: ILoginModel;
	fields?: IFormField[];
}

export default function ForgotPassword() {

	const metadataService = new MetadataService(),
		accountService = new AccountService();

	const { t } = useTranslation("idx"),
		[state, setState] = React.useState<IState>({ loading: true }),
		[email, setEmail] = useLocalStorage(StorageNames.email, "");

	React.useEffect(() => {
		async function fetchData() {
			const dataView = await metadataService.load(Views.formForgotPassword);

			setState({ loading: false, fields: dataView.fields });
		}

		fetchData();

		return async () => {
			await metadataService.abort();
			await accountService.abort();
		};
	}, []);

	async function handleChange(values: ILoginModel) {
		setEmail(values.email);
	};

	async function handleSubmit(values: ILoginModel): Promise<IApiResult> {
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

			<p><Link to={Patterns.login}><Icon type="arrow-left" /> {t("page.register.link.login")}</Link></p>

		</Page>
	);
}
