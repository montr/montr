import { Fetcher } from "./fetcher";
import { Constants } from "..";
import { IDataView, IPaneProps, IDataField, IApiResult, Guid, IFieldType } from "../models";

interface IManageFieldDataRequest {
	entityTypeCode: string;
	entityUid: Guid | string;
	item: IDataField;
}

interface IDeleteFieldDataRequest {
	entityTypeCode: string;
	entityUid: Guid | string;
	uids: string[] | number[];
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

	get = async (entityTypeCode: string, entityUid: Guid | string, uid: Guid): Promise<IDataField> => {
		return this.post(`${Constants.apiURL}/metadata/get`, { entityTypeCode, entityUid, uid });
	};

	insert = async (request: IManageFieldDataRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/insert`, request);
	};

	update = async (request: IManageFieldDataRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/update`, request);
	};

	delete = async (request: IDeleteFieldDataRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/delete`, request);
	};
};
