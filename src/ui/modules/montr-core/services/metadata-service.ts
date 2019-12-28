import { Fetcher } from "./fetcher";
import { Constants } from "..";
import { IDataView, IPaneProps, IDataField, IApiResult, Guid, IFieldType } from "../models";

interface IInsertDataFieldRequest {
	entityTypeCode: string;
	item: IDataField;
}

export class MetadataService extends Fetcher {

	load = async<TEntity>(viewId: string, componentToClass?: (component: string) => React.ComponentClass): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.apiURL}/metadata/view`, { viewId: viewId });

		if (componentToClass) {
			data.panes && data.panes.forEach((pane) => {
				if (pane.component) {
					pane.component = componentToClass(pane.component.toString()) as React.ComponentClass<IPaneProps<TEntity>>;
				}
			});
		}

		return data;
	};

	fieldTypes = async (entityTypeCode: string): Promise<IFieldType[]> => {
		return this.post(`${Constants.apiURL}/metadata/fieldTypes`, { entityTypeCode });
	};

	get = async (entityTypeCode: string, uid: Guid): Promise<IDataField> => {
		return this.post(`${Constants.apiURL}/metadata/get`, { entityTypeCode, uid });
	};

	insert = async (request: IInsertDataFieldRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/insert`, request);
	};

	update = async (entityTypeCode: string, item: IDataField): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/update`, { entityTypeCode, item });
	};

	delete = async (entityTypeCode: string, uids: string[] | number[]): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/delete`, { entityTypeCode, uids });
	};
};
