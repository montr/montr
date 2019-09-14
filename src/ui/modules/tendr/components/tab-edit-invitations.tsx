import * as React from "react";
import { Button, Icon, Drawer, Alert } from "antd";
import { IPaneProps, Guid, IDataResult } from "@montr-core/models";
import { IPaneComponent, DataTable, Toolbar, DataTableUpdateToken } from "@montr-core/components";
import { PaneSearchClassifier } from "@montr-master-data/components";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { ModalEditInvitation } from "../components";
import { IEvent, IInvitation } from "../models";
import { InvitationService } from "../services";
import { Constants } from "@montr-core/.";

interface IProps extends CompanyContextProps, IPaneProps<IEvent> {
	data: IEvent;
}

interface IState {
	showDrawer?: boolean;
	modalData?: IInvitation;
	updateTableToken: DataTableUpdateToken;
}

class _TabEditInvitations extends React.Component<IProps, IState> {

	_invitationService = new InvitationService();

	private _formRef: IPaneComponent;

	constructor(props: IProps) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentWillUnmount = async () => {
		await this._invitationService.abort();
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { currentCompany, data } = this.props;

		if (currentCompany && data) {

			const params = {
				companyUid: currentCompany.uid,
				eventUid: data.uid,
				...postParams
			};

			return await this._invitationService.post(loadUrl, params);
		}

		return null;
	}

	save() {
		this._formRef.save();
	}

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	}

	showAddDrawer = () => {
		this.setState({ showDrawer: true });
	};

	onCloseDrawer = () => {
		this.setState({ showDrawer: false });
	};

	onSelect = async (keys: string[]) => {
		const { data, currentCompany } = this.props;

		if (currentCompany) {
			await this._invitationService.insert(currentCompany.uid, {
				eventUid: data.uid,
				items: keys.map(x => {
					return { counterpartyUid: new Guid(x) };
				})
			});

			this.onCloseDrawer();

			await this.refreshTable();
		}
	}

	showAddModal = () => {
		this.setState({ modalData: {} });
	}

	onModalSuccess = async (data: IInvitation) => {
		this.setState({ modalData: null });

		await this.refreshTable();
	}

	onModalCancel = () => {
		this.setState({ modalData: null });
	}

	render() {
		const { modalData, updateTableToken, showDrawer } = this.state;

		return <>
			<Toolbar>
				<Button onClick={this.showAddDrawer} type="primary"><Icon type="plus" /> Пригласить</Button>
				<Button onClick={this.showAddModal}><Icon type="plus" /> Добавить</Button>
			</Toolbar>

			<div style={{ clear: "both" }} />

			<DataTable
				viewId="PrivateEventCounterpartyList/Grid"
				loadUrl={`${Constants.apiURL}/invitation/list/`}
				onLoadData={this.onLoadTableData}
				updateToken={updateTableToken}
			/>

			<p />

			<Alert type="info" message={
				<ul>
					<li>Manual add</li>
					<li>Import from *.xls etc</li>
					<li>Select from registered companies</li>
					<li>Invite from companies catalogs</li>
					<li>👍 Select from counterparty classifier</li>
					<li>Copy invitation from other event</li>
				</ul>
			} />

			{modalData &&
				<ModalEditInvitation
					onSuccess={this.onModalSuccess}
					onCancel={this.onModalCancel}
				/>}

			{showDrawer &&
				<Drawer
					// title="Контрагенты"
					closable={false}
					onClose={this.onCloseDrawer}
					visible={true}
					width={1024}
				>
					<PaneSearchClassifier
						mode="Drawer"
						typeCode="counterparty"
						onSelect={this.onSelect}
					/>
				</Drawer>}
		</>;
	}
}

export const TabEditInvitations = withCompanyContext(_TabEditInvitations);
