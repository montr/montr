import * as React from "react";
import { DataTable, DataTableUpdateToken, Toolbar, ButtonAdd } from "@montr-core/components";
import { IClassifierType, IClassifierTree } from "../models";
import { Constants } from "@montr-core/.";
import { ClassifierTreeService } from "../services";
import { DataResult, IMenu } from "@montr-core/models";
import { Alert, Modal } from "antd";
import { ModalEditClassifierTree } from ".";

interface Props {
	type: IClassifierType;
}

interface State {
	editData?: IClassifierTree;
	updateTableToken: DataTableUpdateToken;
}

export class TabEditClassifierTypeHierarchy extends React.Component<Props, State> {

	private _classifierTreeService = new ClassifierTreeService();

	constructor(props: Props) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentDidUpdate = async (prevProps: Props) => {
		if (this.props.type !== prevProps.type) {
			await this.refreshTable();
		}
	};

	componentWillUnmount = async () => {
		await this._classifierTreeService.abort();
	};

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {
		const { type } = this.props;

		if (type.code) {

			const params = {
				typeCode: type.code,
				...postParams
			};

			return await this._classifierTreeService.post(loadUrl, params);
		}

		return null;
	};

	showAddGroupModal = () => {
		this.setState({ editData: {} });
	};

	showEditModal = (data: IClassifierTree) => {
		this.setState({ editData: data });
	};

	showDeleteConfirm = (data: IClassifierTree) => {
		Modal.confirm({
			title: "Вы действительно хотите удалить выбранную иерархию?",
			content: "Наверняка что-то случится с группами и элементами классификатора...",
			onOk: async () => {
				const { type } = this.props;

				await this._classifierTreeService.delete(type.code, [data.uid]);

				this.refreshTable();
			}
		});
	};

	onGroupModalSuccess = async (data: IClassifierTree) => {
		this.setState({ editData: null });

		await this.refreshTable();
	};

	onGroupModalCancel = () => {
		this.setState({ editData: null });
	};

	render() {
		const { type } = this.props,
			{ editData: groupEditData, updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Редактировать", onClick: this.showEditModal },
			{ name: "Удалить", onClick: this.showDeleteConfirm }
		];

		return (<>
			<Alert type={type.hierarchyType == "Groups" ? "info" : "warning"}
				message="Настройка иерархий групп доступна для классификторов, у которых на вкладке «Информация» выбран тип иерархии «Группы»." />

			{type.hierarchyType == "Groups" && (<>
				<Toolbar clear>
					<ButtonAdd onClick={this.showAddGroupModal} />
				</Toolbar>

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
