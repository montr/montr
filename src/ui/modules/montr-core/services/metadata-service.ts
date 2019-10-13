import { Fetcher } from "./fetcher";
import { Constants } from "..";
import { IDataView, IPaneProps } from "../models";

export class MetadataService extends Fetcher {

	load = async<TEntity>(viewId: string, componentToClass?: (component: string) => React.ComponentClass): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.apiURL}/Metadata/View`, { viewId: viewId });

		if (componentToClass) {
			data.panes && data.panes.forEach((pane) => {
				if (pane.component) {
					pane.component = componentToClass(pane.component.toString()) as React.ComponentClass<IPaneProps<TEntity>>;
				}
			});
		}

		return data;
	}

};
