import * as React from "react";
import { Button, Icon } from "antd";
import { IPaneProps } from "@montr-core/models";
import { IEvent } from "../../models";
import { IPaneComponent, DataTable, Toolbar } from "@montr-core/components";

interface IEditEventPaneProps extends IPaneProps<IEvent> {
	data: IEvent;
}

export class InvitationPane extends React.Component<IEditEventPaneProps> {

	private _formRef: IPaneComponent;

	save() {
		this._formRef.save();
	}

	showAddModal = () => {
		this.setState({ modalData: {} });
	}

	render() {
		return <>
			<ol>
				<li>Manual add</li>
				<li>Import from *.xls etc</li>
				<li>Select from registered companies</li>
				<li>Invite from companies catalogs</li>
				<li>Select from counterparty classifier</li>
				<li>Copy invitation from other event</li>
			</ol>

			<Toolbar>
				<Button onClick={this.showAddModal}><Icon type="plus" /> Добавить</Button>
			</Toolbar>

			<div style={{ clear: "both" }} />

			<DataTable
				viewId="PrivateEventCounterpartyList/Grid"
				loadUrl="/api/Company/List" />
		</>;
	}
}
