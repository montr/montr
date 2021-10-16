import { ButtonAdd, DataTable, DataTableUpdateToken, Toolbar } from "@montr-core/components";
import { DataResult, IMenu } from "@montr-core/models";
import { Alert, Modal } from "antd";
import * as React from "react";
import { ModalEditClassifierTree } from ".";
import { ClassifierTree, ClassifierType } from "../models";
import { Api } from "../module";
import { ClassifierTreeService } from "../services";

interface Props {
	data: ClassifierType;
}

interface State {
	editData?: ClassifierTree;
	updateTableToken: DataTableUpdateToken;
}

export default class TabEditClassifierTypeHierarchy extends React.Component<Props, State> {

	private _classifierTreeService = new ClassifierTreeService();

	constructor(props: Props) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.data !== prevProps.data) {
			await this.refreshTable();
		}
	};

	componentWillUnmount = async (): Promise<void> => {
		await this._classifierTreeService.abort();
	};

	refreshTable = async (resetSelectedRows?: boolean): Promise<void> => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<unknown>> => {
		const { data: type } = this.props;

		if (type.code) {

			const params = {
				typeCode: type.code,
				...postParams
			};

			return await this._classifierTreeService.post(loadUrl, params);
		}

		return null;
	};

	showAddGroupModal = (): void => {
		this.setState({ editData: {} });
	};

	showEditModal = (data: ClassifierTree): void => {
		this.setState({ editData: data });
	};

	showDeleteConfirm = (data: ClassifierTree): void => {
		Modal.confirm({
			title: "Вы действительно хотите удалить выбранную иерархию?",
			content: "Наверняка что-то случится с группами и элементами классификатора...",
			onOk: async () => {
				const { data: type } = this.props;

				await this._classifierTreeService.delete(type.code, [data.uid]);

				this.refreshTable();
			}
		});
	};

	onGroupModalSuccess = async (data: ClassifierTree): Promise<void> => {
		this.setState({ editData: null });

		await this.refreshTable();
	};

	onGroupModalCancel = (): void => {
		this.setState({ editData: null });
	};

	render = (): React.ReactNode => {
		const { data: type } = this.props,
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
					loadUrl={Api.classifierTreeList}
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
	};
}
