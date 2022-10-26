import { DataForm, Icon, Page, useLocalStorage } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Spin } from "antd";
import * as React from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { SendEmailConfirmationModel } from "../models";
import { Locale, Patterns, StorageNames, Views } from "../module";
import { AccountService } from "../services/account-service";

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default function SendEmailConfirmation() {

	const accountService = new AccountService();

	const { t } = useTranslation(Locale.Namespace),
		[state, setState] = React.useState<State>({ loading: true }),
		[email, setEmail] = useLocalStorage(StorageNames.email, "");

	React.useEffect(() => {
		const metadataService = new MetadataService();

		async function fetchData() {
			const dataView = await metadataService.load(Views.formSendEmailConfirmation);

			setState({ loading: false, fields: dataView.fields });
		}

		fetchData();

		return () => {
			metadataService.abort();
			accountService.abort();
		};
	}, []);

	async function handleChange(changedValues: SendEmailConfirmationModel, values: SendEmailConfirmationModel) {
		setEmail(values.email);
	}

	async function handleSubmit(values: SendEmailConfirmationModel): Promise<ApiResult> {
		return await accountService.sendEmailConfirmation(values);
	}

	const { loading, fields } = state;

	return (
		<Page title={t("page.sendEmailConfirmation.title") as string}>

			<p>{t("page.sendEmailConfirmation.subtitle") as string}</p>

			<Spin spinning={loading}>
				<DataForm
					fields={fields}
					data={{ email }}
					onValuesChange={handleChange}
					onSubmit={handleSubmit}
					submitButton={t("button.sendEmailConfirmation")}
				/>
			</Spin>

			<p><Link to={Patterns.login}>{Icon.ArrowLeft} {t("page.register.link.login") as string}</Link></p>

		</Page>
	);
}
