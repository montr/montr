import React from "react";
import { DataTableUpdateToken, Page, DataTable, Toolbar, PageHeader, DataBreadcrumb, ButtonImport, ButtonExport, Icon } from ".";
import { Constants } from "..";
import { IMenu, ILocaleString, IDataResult } from "../models";
import { LocaleStringService, NotificationService } from "../services";
import { Form, Select, Button, Upload } from "antd";
import { UploadChangeParam } from "antd/lib/upload";
import { Translation } from "react-i18next";

interface IProps {
}

interface IState {
	locale?: string;
	module?: string;
	updateTableToken: DataTableUpdateToken;
}

export default class SearchLocaleString extends React.Component<IProps, IState> {

	_notification = new NotificationService();
	_localeService = new LocaleStringService();

	constructor(props: IProps) {
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

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {

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

	render = () => {
		const { locale, module, updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Редактировать", route: (item: ILocaleString) => item.key },
			{ name: "Удалить", route: (item: ILocaleString) => item.key },
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
									action={`${Constants.apiURL}/locale/import/`}
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
								<Button type="primary" htmlType="submit" icon={Icon.Search}>{t("button.search")}</Button>
							</Form.Item>
						</Form>

						<br />

						<DataTable
							rowKey={(x: ILocaleString) => `${x.locale}-${x.module}-${x.key}`}
							rowActions={rowActions}
							viewId={`LocaleString/Grid/`}
							loadUrl={`${Constants.apiURL}/locale/list/`}
							onLoadData={this.onLoadTableData}
							updateToken={updateTableToken}
						/>

					</Page>
				)}
			</Translation>
		);
	};
};
