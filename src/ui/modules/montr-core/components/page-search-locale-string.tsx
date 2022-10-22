import { Api } from "@montr-core/module";
import { Button, Form, Select, Upload } from "antd";
import { UploadChangeParam } from "antd/lib/upload";
import React from "react";
import { Translation } from "react-i18next";
import { ButtonExport, ButtonImport, DataBreadcrumb, DataTable, DataTableUpdateToken, Icon, Page, PageHeader, Toolbar } from ".";
import { DataResult, IMenu, LocaleString } from "../models";
import { LocaleStringService, NotificationService } from "../services";

interface Props {
}

interface State {
	locale?: string;
	module?: string;
	updateTableToken: DataTableUpdateToken;
}

export default class PageSearchLocaleString extends React.Component<Props, State> {

	_notification = new NotificationService();
	_localeService = new LocaleStringService();

	constructor(props: Props) {
		super(props);

		this.state = {
			// locale: "en",
			// module: "common",
			updateTableToken: { date: new Date() }
		};
	}

	componentWillUnmount = async () => {
		await this._localeService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {

		const params = {
			locale: this.state.locale,
			module: this.state.module,
			...postParams
		};

		return await this._localeService.post(loadUrl, params);
	};

	handleSubmit = async (values: any) => {
		this.setState({
			locale: values.locale,
			module: values.module
		});

		await this.refreshTable();
	};

	handleImport = async (e: React.SyntheticEvent) => {
	};

	handleExport = async (e: React.SyntheticEvent) => {
		const { locale, module } = this.state;

		await this._localeService.export({ locale, module });
	};

	handleUploadChange = async (info: UploadChangeParam) => {
		if (info.file.status === "done") {
			this._notification.success(`File "${info.file.name}" uploaded successfully.`);

			this.refreshTable();
		} else if (info.file.status === "error") {
			this._notification.error(`File "${info.file.name}" upload failed.`);
		}
	};

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	render = (): React.ReactNode => {
		const { locale, module, updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Редактировать", route: (item: LocaleString) => item.key },
			{ name: "Удалить", route: (item: LocaleString) => item.key },
		];

		const locales = ["en", "ru"],
			modules = ["common", "idx", "master-data", "tendr"];

		return (
			<Translation>
				{(t) => (
					<Page
						title={<>
							<Toolbar float="right">
								<Upload
									accept=".json"
									action={Api.localeImport}
									name="file"
									showUploadList={false}
									style={{ display: "inline-block" }}
									// customRequest={this.handleUpload}
									onChange={this.handleUploadChange}>
									<ButtonImport onClick={this.handleImport} />
								</Upload>
								<ButtonExport onClick={this.handleExport} />
							</Toolbar>

							<DataBreadcrumb items={[{ name: "Локализация" }]} />
							<PageHeader>Локализация</PageHeader>
						</>}>

						<Form layout="inline" onFinish={this.handleSubmit}>
							<Form.Item name="locale" rules={[{ required: false }]}>
								<Select placeholder="Язык" allowClear value={locale} style={{ minWidth: 100 }}>
									{locales.map(x => {
										return <Select.Option key={`${x}`} value={`${x}`}>{x}</Select.Option>;
									})}
								</Select>
							</Form.Item>
							<Form.Item name="module" rules={[{ required: false }]}>
								<Select placeholder="Модуль" allowClear value={module} style={{ minWidth: 100 }}>
									{modules.map(x => {
										return <Select.Option key={`${x}`} value={`${x}`}>{x}</Select.Option>;
									})}
								</Select>
							</Form.Item>
							<Form.Item>
								<Button type="primary" htmlType="submit" icon={Icon.Search}>{t("button.search") as string}</Button>
							</Form.Item>
						</Form>

						<br />

						<DataTable
							rowKey={(x: LocaleString) => `${x.locale}-${x.module}-${x.key}`}
							rowActions={rowActions}
							viewId={`LocaleString/Grid/`}
							loadUrl={Api.localeList}
							onLoadData={this.onLoadTableData}
							updateToken={updateTableToken}
						/>

					</Page>
				)}
			</Translation>
		);
	};
}
