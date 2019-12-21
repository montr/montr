import * as React from "react";
import { Page, PageHeader, Toolbar, DataTable, DataTableUpdateToken, Icon } from "@montr-core/components";
import { Constants } from "@montr-core/.";
import { withCompanyContext, CompanyContextProps } from "@montr-kompany/components";
import { ClassifierBreadcrumb } from "../components";
import { Link } from "react-router-dom";
import { Button } from "antd";
import { ClassifierTypeService } from "../services";
import { NotificationService } from "@montr-core/services";
import { IMenu } from "@montr-core/models";
import { IClassifierGroup } from "@montr-master-data/models";
import { RouteBuilder } from "../module";

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

	componentDidUpdate = async (prevProps: IProps) => {
		// todo: detect company changed without CompanyContextProps (here and everywhere)
		if (this.props.currentCompany !== prevProps.currentCompany) {
			this.refreshTable(true);
		}
	};

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	};

	// todo: show confirm
	// todo: localize
	delete = async () => {
		try {
			const rowsAffected = await this._classifierTypeService.delete(this.state.selectedRowKeys);

			this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

			this.refreshTable(true);
		} catch (error) {
			this._notificationService.error("Ошибка при удалении данных", error.message);
		}
	};

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	render = () => {
		const { updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Настроить", route: (item: IClassifierGroup) => RouteBuilder.editClassifierType(item.uid) }
		];

		return (
			// todo: localize
			<Page
				title={<>
					<Toolbar float="right">
						<Link to={`/classifiers/add`}>
							<Button type="primary" icon={Icon.Plus}>Добавить</Button>
						</Link>
						<Button icon={Icon.Delete} onClick={this.delete}> Удалить</Button>
					</Toolbar>

					<ClassifierBreadcrumb />
					<PageHeader>Справочники</PageHeader>
				</>}>

				<DataTable
					rowKey="uid"
					viewId={`ClassifierType/Grid/`}
					loadUrl={`${Constants.apiURL}/classifierType/list/`}
					onSelectionChange={this.onSelectionChange}
					updateToken={updateTableToken}
					rowActions={rowActions}
				/>

			</Page>
		);
	};
}

const SearchClassifierType = withCompanyContext(_SearchClassifierType);

export default SearchClassifierType;
