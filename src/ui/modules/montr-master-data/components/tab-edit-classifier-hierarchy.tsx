import { ButtonAdd, DataTable, DataTableUpdateToken, Toolbar } from "@montr-core/components";
import { DataResult, Guid, IMenu } from "@montr-core/models";
import { Alert, Modal } from "antd";
import * as React from "react";
import { ModalEditClassifierLink } from ".";
import { Classifier, ClassifierLink, ClassifierType } from "../models";
import { Api } from "../module";
import { ClassifierLinkService } from "../services";

interface Props {
	type: ClassifierType;
	data: Classifier;
	onDataChange?: (values: Classifier) => void;
}

interface State {
	modalData?: ClassifierLink;
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

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.type !== prevProps.type ||
			this.props.data !== prevProps.data) {
			await this.refreshTable();
		}
	};

	componentWillUnmount = async (): Promise<void> => {
		await this._classifierLinkService.abort();
	};

	refreshTable = async (resetSelectedRows?: boolean): Promise<void> => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<unknown>> => {
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

	showAddLinkModal = (): void => {
		this.setState({ modalData: {} });
	};

	showDeleteLinkConfirm = (data: ClassifierLink): void => {
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

	onModalSuccess = async (data: ClassifierLink): Promise<void> => {
		this.setState({ modalData: null });

		await this.refreshTable();
	};

	onModalCancel = (): void => {
		this.setState({ modalData: null });
	};

	render = (): React.ReactNode => {
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
					rowKey={(x: ClassifierLink) => `${x.tree.uid}`}
					viewId="ClassifierLink/Grid"
					loadUrl={Api.classifierLinkList}
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
	};
}
