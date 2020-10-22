import { Fetcher } from "./fetcher";
import { Constants } from "..";
import { DataView, IDataField, ApiResult, Guid, IFieldType } from "../models";

interface IManageFieldDataRequest {
	entityTypeCode: string;
	entityUid: Guid;
	item: IDataField;
}

interface IDeleteFieldDataRequest {
	entityTypeCode: string;
	entityUid: Guid;
	uids: string[] | number[];
}

export class MetadataService extends Fetcher {

	load = async<TEntity>(viewId: string): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(`${Constants.apiURL}/metadata/view`, { viewId: viewId });

		return data;
	};

	fieldTypes = async (entityTypeCode: string): Promise<IFieldType[]> => {
		return this.post(`${Constants.apiURL}/metadata/fieldTypes`, { entityTypeCode });
	};

	get = async (entityTypeCode: string, entityUid: Guid, uid: Guid): Promise<IDataField> => {
		return this.post(`${Constants.apiURL}/metadata/get`, { entityTypeCode, entityUid, uid });
	};

	insert = async (request: IManageFieldDataRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/insert`, request);
	};

	update = async (request: IManageFieldDataRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/update`, request);
	};

	delete = async (request: IDeleteFieldDataRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/metadata/delete`, request);
	};
};
