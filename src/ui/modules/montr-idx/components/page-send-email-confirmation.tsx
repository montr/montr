import * as React from "react";
import { Spin } from "antd";
import { useTranslation } from "react-i18next";
import { useLocalStorage, Page, DataForm, Icon } from "@montr-core/components";
import { IDataField, ApiResult } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { ISendEmailConfirmationModel } from "../models";
import { AccountService } from "../services/account-service";
import { Views, Patterns, StorageNames } from "../module";
import { Link } from "react-router-dom";

interface IState {
	loading: boolean;
	fields?: IDataField[];
}

export default function SendEmailConfirmation() {

	const accountService = new AccountService();

	const { t } = useTranslation("idx"),
		[state, setState] = React.useState<IState>({ loading: true }),
		[email, setEmail] = useLocalStorage(StorageNames.email, "");

	React.useEffect(() => {
		const metadataService = new MetadataService();

		async function fetchData() {
			const dataView = await metadataService.load(Views.formSendEmailConfirmation);

			setState({ loading: false, fields: dataView.fields });
		}

		fetchData();

		return async () => {
			await metadataService.abort();
			// await accountService.abort();
		};
	}, []);

	async function handleChange(values: ISendEmailConfirmationModel) {
		setEmail(values.email);
	};

	async function handleSubmit(values: ISendEmailConfirmationModel): Promise<ApiResult> {
		return await accountService.sendEmailConfirmation(values);
	};

	const { loading, fields } = state;

	return (
		<Page title={t("page.sendEmailConfirmation.title")}>

			<p>{t("page.sendEmailConfirmation.subtitle")}</p>

			<Spin spinning={loading}>
				<DataForm
					fields={fields}
					data={{ email }}
					onChange={handleChange}
					onSubmit={handleSubmit}
					submitButton={t("button.sendEmailConfirmation")}
				/>
			</Spin>

			<p><Link to={Patterns.login}>{Icon.ArrowLeft} {t("page.register.link.login")}</Link></p>

		</Page>
	);
}
