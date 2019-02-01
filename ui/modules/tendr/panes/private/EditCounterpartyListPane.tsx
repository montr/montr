import * as React from "react";
import { IPaneProps } from "@montr-core/models";
import { IEvent } from "../../api";
import { IPaneComponent, DataTable } from "@montr-core/components";

interface IEditEventPaneProps extends IPaneProps<IEvent> {
	data: IEvent;
}

export class EditCounterpartyListPane extends React.Component<IEditEventPaneProps> {

	private _formRef: IPaneComponent;

	save() {
		this._formRef.save();
	}

	render() {
		return (
			<DataTable
				viewId="PrivateEventCounterpartyList/Grid"
				loadUrl="/api/Company/List" />
		);
	}
}
