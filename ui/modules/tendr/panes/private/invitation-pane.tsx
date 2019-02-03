import * as React from "react";
import { IPaneProps } from "@montr-core/models";
import { IEvent } from "../../models";
import { IPaneComponent, DataTable } from "@montr-core/components";

interface IEditEventPaneProps extends IPaneProps<IEvent> {
	data: IEvent;
}

export class InvitationPane extends React.Component<IEditEventPaneProps> {

	private _formRef: IPaneComponent;

	save() {
		this._formRef.save();
	}

	render() {
		return <>
			<ol>
				<li>Ручное добавление</li>
				<li>Импорт xsl и т.д.</li>
				<li>Выбор из зарегистрированных в системе</li>
				<li>Выбор из своих контрагентов</li>
				<li>Копирование приглашений из другой процедуры</li>
			</ol>

			<DataTable
				viewId="PrivateEventCounterpartyList/Grid"
				loadUrl="/api/Company/List" />
		</>;
	}
}
