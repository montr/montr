import { Fetcher } from ".";
import { IDataView, IPaneProps } from "../models";
import { Constants } from "..";

import * as panes from "../../tendr/panes/private"

const componentToClass: Map<string, React.ComponentClass> = new Map<string, React.ComponentClass>();
componentToClass.set("panes/private/EditEventPane", panes.EditEventPane);
componentToClass.set("panes/private/InvitationPane", panes.InvitationPane);

export class MetadataService extends Fetcher {

	load = async<TEntity>(viewId: string): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.baseURL}/Metadata/View`, { viewId: viewId });

		data.panes && data.panes.forEach((pane) => {
			if (pane.component) {
				pane.component = componentToClass
					.get(pane.component.toString()) as React.ComponentClass<IPaneProps<TEntity>>;
			}
		});

		return data;
	}

};
