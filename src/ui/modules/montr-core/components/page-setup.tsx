import { SetupService } from "@montr-core/services/setup-service";
import { Alert, Spin } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { DataForm, Page } from ".";
import { Constants } from "..";
import { ApiResult, AppState, IDataField, IIndexer } from "../models";
import { Views } from "../module";
import { MetadataService } from "../services";

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default class PageSetup extends React.Component<unknown, State> {

	private readonly metadataService = new MetadataService();
	private readonly setupService = new SetupService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.metadataService.abort();
		await this.setupService.abort();
	};

	fetchData = async (): Promise<void> => {
		const dataView = await this.metadataService.load(Views.setupForm);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: IIndexer): Promise<ApiResult> => {
		return await this.setupService.save(values);
	};

	render = (): React.ReactNode => {
		const { loading, fields } = this.state,
			appState = Constants.appState,
			data = {};

		if (appState == AppState.Initialized) {
			return (
				<Translation>
					{(t) => <Page title={t("page.setup.title") as string}>

						<Alert type="warning" message={t("page.setup.initializedMessage") as string} />

					</Page>}
				</Translation>
			);
		}

		return (
			<Translation>
				{(t) => <Page title={t("page.setup.title") as string}>

					<p>{t("page.setup.subtitle") as string}</p>

					<Spin spinning={loading}>
						<DataForm
							layout="vertical"
							fields={fields}
							data={data}
							onSubmit={this.save}
						/>
					</Spin>

				</Page>}
			</Translation>
		);
	};
}
