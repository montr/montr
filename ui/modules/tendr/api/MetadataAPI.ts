import { Fetcher } from "@montr-core/services";

import { IDataView, IPaneProps } from "./";
import { Constants } from "./Constants";

import * as panes from "../panes/private"

const getLoadUrl = (): string => {
	return `${Constants.baseURL}/Metadata/View`;
}

const componentToClass: Map<string, React.ComponentClass> = new Map<string, React.ComponentClass>();
componentToClass.set("panes/private/EditEventPane", panes.EditEventPane);

const load = async<TEntity>(viewId: string): Promise<IDataView<TEntity>> => {
	const data: IDataView<TEntity> = await Fetcher.post(getLoadUrl(), { viewId: viewId });

	data.panes && data.panes.forEach((pane) => {
		if (pane.component) {
			pane.component = componentToClass
				.get(pane.component.toString()) as React.ComponentClass<IPaneProps<TEntity>>;
		}
	});

	return data;
};

export const MetadataAPI = {
	getLoadUrl, load
};
