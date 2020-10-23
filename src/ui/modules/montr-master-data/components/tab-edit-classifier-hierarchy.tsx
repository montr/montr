import * as React from "react";
import { Alert, Modal } from "antd";
import { Constants } from "@montr-core/.";
import { DataTableUpdateToken, Toolbar, DataTable, ButtonAdd } from "@montr-core/components";
import { IClassifierType, IClassifier, IClassifierLink } from "../models";
import { DataResult, IMenu, Guid } from "@montr-core/models";
import { ClassifierLinkService } from "../services";
import { ModalEditClassifierLink } from ".";

interface Props {
	type: IClassifierType;
	data: IClassifier;
	onDataChange?: (values: IClassifier) => void;
}

interface State {
	modalData?: IClassifierLink;
	updateTableToken: DataTableUpdateToken;
}

export default class TabEditClassifierHierarchy extends React.Component<Props, State> {

	_classifierLinkService = new ClassifierLinkService();

	constructor(props: Props) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentDidUpdate = async (prevProps: Props) => {
		if (this.props.type !== prevProps.type ||
			this.props.data !== prevProps.data) {
			await this.refreshTable();
		}
	};

	componentWillUnmount = async () => {
		await this._classifierLinkService.abort();
	};

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {
		const { type, data } = this.props;

		if (type.code) {

			const params = {
				typeCode: type.code,
				itemUid: data.uid,
				...postParams
			};

			return await this._classifierLinkService.post(loadUrl, params);
		}

		return null;
	};

	showAddLinkModal = () => {
		this.setState({ modalData: {} });
	};

	showDeleteLinkConfirm = (data: IClassifierLink) => {
		Modal.confirm({
			title: "Вы действительно хотите удалить связь с выбранной группой?",
			content: "При удалении связи с группой иерархии по-умолчанию, элемент будет привязан к корню иерархии по-умолчанию.",
			onOk: async () => {
				const { type } = this.props;

				await this._classifierLinkService.delete(type.code, data.group.uid, data.item.uid);

				this.refreshTable();
			}
		});
	};

	onModalSuccess = async (data: IClassifierLink) => {
		this.setState({ modalData: null });

		await this.refreshTable();
	};

	onModalCancel = () => {
		this.setState({ modalData: null });
	};

	render() {
		const { type, data } = this.props,
			{ modalData, updateTableToken } = this.state;

		if (!type) return null;

		const rowActions: IMenu[] = [
			{ name: "Удалить", onClick: this.showDeleteLinkConfirm }
		];

		return (<>
			<Alert type={type.hierarchyType == "Groups" ? "info" : "warning"}
				message="Настройка иерархий групп доступна для типов классификаторов, у которых на вкладке «Информация» выбран тип иерархии «Группы»." />

			{type.hierarchyType == "Groups" && (<>
				<Toolbar clear>
					<ButtonAdd onClick={this.showAddLinkModal} />
				</Toolbar>

				<DataTable
					rowKey={(x: IClassifierLink) => `${x.tree.uid}`}
					viewId="ClassifierLink/Grid"
					loadUrl={`${Constants.apiURL}/classifierLink/list/`}
					rowActions={rowActions}
					onLoadData={this.onLoadTableData}
					updateToken={updateTableToken}
				/>

				{modalData &&
					<ModalEditClassifierLink
						typeCode={type.code}
						itemUid={new Guid(data.uid.toString())}
						onSuccess={this.onModalSuccess}
						onCancel={this.onModalCancel}
					/>}
			</>)}
		</>);
	}
}
