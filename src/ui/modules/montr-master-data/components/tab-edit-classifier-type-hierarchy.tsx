import * as React from "react";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { DataTable, DataTableUpdateToken, Toolbar } from "@montr-core/components";
import { IClassifierType, IClassifierGroup } from "../models";
import { Constants } from "@montr-core/.";
import { ClassifierGroupService } from "../services";
import { IDataResult, IMenu } from "@montr-core/models";
import { Alert, Button, Icon, Modal } from "antd";
import { ModalEditClassifierGroup } from ".";

interface IProps extends CompanyContextProps {
	type: IClassifierType;
}

interface IState {
	groupEditData?: IClassifierGroup;
	updateTableToken: DataTableUpdateToken;
}

class _TabEditClassifierTypeHierarchy extends React.Component<IProps, IState> {

	_classifierGroupService = new ClassifierGroupService();

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
		await this._classifierGroupService.abort();
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

			return await this._classifierGroupService.post(loadUrl, params);
		}

		return null;
	}

	showAddGroupModal = () => {
		this.setState({ groupEditData: {} });
	}

	showEditGroupModal = (data: IClassifierGroup) => {
		this.setState({ groupEditData: data });
	}

	showDeleteGroupConfirm = (data: IClassifierGroup) => {
		Modal.confirm({
			title: "Вы действительно хотите удалить выбранную группу?",
			content: "Дочерние группы и элементы будут перенесены к родительской группе.",
			onOk: async () => {
				const { currentCompany, type } = this.props

				await this._classifierGroupService.delete(currentCompany.uid, type.code, data.uid);

				this.refreshTable();
			}
		});
	}

	onGroupModalSuccess = async (data: IClassifierGroup) => {
		this.setState({ groupEditData: null });

		await this.refreshTable();
	}

	onGroupModalCancel = () => {
		this.setState({ groupEditData: null });
	}

	render() {
		const { type } = this.props,
			{ groupEditData, updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Редактировать", onClick: this.showEditGroupModal },
			{ name: "Удалить", onClick: this.showDeleteGroupConfirm }
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
					loadUrl={`${Constants.baseURL}/classifierGroup/list/`}
					rowActions={rowActions}
					onLoadData={this.onLoadTableData}
					updateToken={updateTableToken}
				/>

				{groupEditData &&
					<ModalEditClassifierGroup
						typeCode={type.code}
						uid={groupEditData.uid}
						hideFields={["parentUid"]}
						onSuccess={this.onGroupModalSuccess}
						onCancel={this.onGroupModalCancel} />}
			</>)}
		</>);
	}
}

export const TabEditClassifierTypeHierarchy = withCompanyContext(_TabEditClassifierTypeHierarchy);
