import React from "react";
import { DataTableUpdateToken, Page, DataTable, Toolbar, PageHeader, DataBreadcrumb } from "@montr-core/components";
import { Constants } from "..";
import { IMenu, ILocaleString, IDataResult } from "@montr-core/models";
import { LocaleStringService } from "@montr-core/services";
import { Form, Select, Button, Icon, Upload, message } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { UploadChangeParam } from "antd/lib/upload";

interface IProps extends FormComponentProps {
}

interface IState {
	locale: string;
	module: string;
	updateTableToken: DataTableUpdateToken;
}

export class _SearchLocaleString extends React.Component<IProps, IState> {

	_localeService = new LocaleStringService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			locale: "en",
			module: "common",
			updateTableToken: { date: new Date() }
		};
	}

	componentWillUnmount = async () => {
		await this._localeService.abort();
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {

		const params = {
			locale: this.state.locale,
			module: this.state.module,
			...postParams
		};

		return await this._localeService.post(loadUrl, params);
	}

	handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		const { form } = this.props;

		form.validateFieldsAndScroll(async (errors, values: any) => {
			if (errors) {
				// console.log(errors);
			} else {
				this.setState({
					locale: values.locale,
					module: values.module
				});

				await this.refreshTable();
			}
		});
	}

	handleImport = async (e: React.SyntheticEvent) => {
	}

	handleExport = async (e: React.SyntheticEvent) => {
		const { locale, module } = this.state;

		await this._localeService.export({ locale, module });
	}

	handleUploadChange = async (info: UploadChangeParam) => {
		if (info.file.status !== 'uploading') {
			console.log(info.file, info.fileList);
		}
		if (info.file.status === 'done') {
			message.success(`${info.file.name} file uploaded successfully`);
		} else if (info.file.status === 'error') {
			message.error(`${info.file.name} file upload failed.`);
		}
	}

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	}

	render = () => {
		const { locale, module, updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Редактировать", route: (item: ILocaleString) => item.key }
		];

		const locales = ["en", "ru"],
			modules = ["common", "master-data", "tendr"];

		const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;

		return (
			<Page
				title={<>
					<Toolbar float="right">
						<Upload showUploadList={false} style={{ display: "inline-block" }} accept=".json"
							onChange={this.handleUploadChange} action="https://www.mocky.io/v2/5cc8019d300000980a055e76">
							<Button onClick={this.handleImport}><Icon type="import" /> Импорт</Button>
						</Upload>
						<Button onClick={this.handleExport}><Icon type="export" /> Экспорт</Button>
					</Toolbar>

					<DataBreadcrumb items={[{ name: "Локализация" }]} />
					<PageHeader>Локализация</PageHeader>
				</>}>

				<Form layout="inline" onSubmit={this.handleSubmit}>
					<Form.Item>
						{getFieldDecorator("locale", {
							rules: [{ required: true }], initialValue: locale
						})(
							<Select placeholder="Локаль" style={{ minWidth: 100 }}>
								{locales.map(x => {
									return <Select.Option key={`${x}`} value={`${x}`}>{x}</Select.Option>
								})}
							</Select>
						)}
					</Form.Item>
					<Form.Item>
						{getFieldDecorator("module", {
							rules: [{ required: true }], initialValue: module
						})(
							<Select placeholder="Модуль" style={{ minWidth: 100 }}>
								{modules.map(x => {
									return <Select.Option key={`${x}`} value={`${x}`}>{x}</Select.Option>
								})}
							</Select>
						)}
					</Form.Item>
					<Form.Item>
						<Button type="primary" htmlType="submit" icon="search">Искать</Button>
					</Form.Item>
				</Form>

				<br />

				<DataTable
					rowKey="key"
					viewId={`LocaleString/Grid/`}
					loadUrl={`${Constants.apiURL}/locale/list/`}
					onLoadData={this.onLoadTableData}
					updateToken={updateTableToken}
					rowActions={rowActions}
				/>

			</Page>
		);
	}
}

export const SearchLocaleString = Form.create<IProps>()(_SearchLocaleString);
