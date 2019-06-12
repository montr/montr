import * as React from "react";
import { Page, PageHeader, Toolbar, DataTable, DataTableUpdateToken } from "@montr-core/components";
import { Constants } from "@montr-core/.";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierBreadcrumb } from "../components";
import { Link } from "react-router-dom";
import { Button, Icon } from "antd";
import { ClassifierTypeService } from "../services";
import { NotificationService } from "@montr-core/services";
import { IDataResult } from "@montr-core/models";

interface IProps extends CompanyContextProps {
}

interface IState {
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

class _SearchClassifierType extends React.Component<IProps, IState> {

	private _classifierTypeService = new ClassifierTypeService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			selectedRowKeys: [],
			updateTableToken: { date: new Date() }
		};
	}

	componentDidMount = async () => {
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.currentCompany !== prevProps.currentCompany) {
			this.refreshTable(true);
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
	}

	onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	}

	// todo: show confirm
	// todo: localize
	delete = async () => {
		try {
			const rowsAffected = await this._classifierTypeService
				.delete(this.props.currentCompany.uid, this.state.selectedRowKeys);

			this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

			this.refreshTable(true);
		} catch (error) {
			this._notificationService.error("Ошибка при удалении данных", error.message);
		}
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { currentCompany } = this.props;

		if (currentCompany) {

			const params = {
				companyUid: currentCompany.uid,
				...postParams
			};

			return await this._classifierTypeService.post(loadUrl, params);
		}

		return null;
	}

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	}

	render = () => {
		const { updateTableToken } = this.state;

		return (
			// todo: localize
			<Page
				title={<>
					<Toolbar float="right">
						<Link to={`/classifiers/add`}>
							<Button type="primary"><Icon type="plus" /> Добавить</Button>
						</Link>
						<Button onClick={this.delete}><Icon type="delete" /> Удалить</Button>
					</Toolbar>

					<ClassifierBreadcrumb />
					<PageHeader>Справочники</PageHeader>
				</>}>

				<DataTable
					rowKey="uid"
					viewId={`ClassifierType/Grid/`}
					loadUrl={`${Constants.baseURL}/classifierType/list/`}
					onLoadData={this.onLoadTableData}
					onSelectionChange={this.onSelectionChange}
					updateToken={updateTableToken}
				/>

			</Page>
		);
	}
}

export const SearchClassifierType = withCompanyContext(_SearchClassifierType);
