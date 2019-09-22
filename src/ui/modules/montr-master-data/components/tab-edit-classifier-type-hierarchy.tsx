import * as React from "react";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { DataTable, DataTableUpdateToken, Toolbar } from "@montr-core/components";
import { IClassifierType, IClassifierTree } from "../models";
import { Constants } from "@montr-core/.";
import { ClassifierTreeService } from "../services";
import { IDataResult, IMenu } from "@montr-core/models";
import { Alert, Button, Icon, Modal } from "antd";
import { ModalEditClassifierTree } from ".";

interface IProps extends CompanyContextProps {
	type: IClassifierType;
}

interface IState {
	editData?: IClassifierTree;
	updateTableToken: DataTableUpdateToken;
}

class _TabEditClassifierTypeHierarchy extends React.Component<IProps, IState> {

	_classifierTreeService = new ClassifierTreeService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.currentCompany !== prevProps.currentCompany ||
			this.props.type !== prevProps.type) {
			await this.refreshTable();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTreeService.abort();
	}

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { currentCompany, type } = this.props;

		if (currentCompany && type.code) {

			const params = {
				companyUid: currentCompany.uid,
				typeCode: type.code,
				...postParams
			};

			return await this._classifierTreeService.post(loadUrl, params);
		}

		return null;
	}

	showAddGroupModal = () => {
		this.setState({ editData: {} });
	}

	showEditModal = (data: IClassifierTree) => {
		this.setState({ editData: data });
	}

	showDeleteConfirm = (data: IClassifierTree) => {
		Modal.confirm({
			title: "Вы действительно хотите удалить выбранную иерархию?",
			content: "Наверняка что-то случится с группами и элементами справочника...",
			onOk: async () => {
				const { type } = this.props

				await this._classifierTreeService.delete(type.code, [data.uid]);

				this.refreshTable();
			}
		});
	}

	onGroupModalSuccess = async (data: IClassifierTree) => {
		this.setState({ editData: null });

		await this.refreshTable();
	}

	onGroupModalCancel = () => {
		this.setState({ editData: null });
	}

	render() {
		const { type } = this.props,
			{ editData: groupEditData, updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Редактировать", onClick: this.showEditModal },
			{ name: "Удалить", onClick: this.showDeleteConfirm }
		];

		return (<>
			<Alert type={type.hierarchyType == "Groups" ? "info" : "warning"}
				message="Настройка иерархий групп доступна для справочников, у которых на вкладке «Информация» выбран тип иерархии «Группы»." />

			{type.hierarchyType == "Groups" && (<>
				<Toolbar>
					<Button onClick={this.showAddGroupModal}><Icon type="plus" /> Добавить</Button>
				</Toolbar>

				<div style={{ clear: "both" }} />

				<DataTable
					rowKey="code"
					viewId="ClassifierType/Grid/Hierarchy"
					// todo: get url from service
					loadUrl={`${Constants.apiURL}/classifierTree/list/`}
					rowActions={rowActions}
					onLoadData={this.onLoadTableData}
					updateToken={updateTableToken}
				/>

				{groupEditData &&
					<ModalEditClassifierTree
						typeCode={type.code}
						uid={groupEditData.uid}
						onSuccess={this.onGroupModalSuccess}
						onCancel={this.onGroupModalCancel}
					/>}
			</>)}
		</>);
	}
}

export const TabEditClassifierTypeHierarchy = withCompanyContext(_TabEditClassifierTypeHierarchy);
