import { ApiResult, DataView, Guid, IDataField, IFieldType } from "../models";
import { Api } from "../module";
import { Fetcher } from "./fetcher";

interface ManageFieldDataRequest {
	entityTypeCode: string;
	entityUid: Guid;
	item: IDataField;
}

interface DeleteFieldDataRequest {
	entityTypeCode: string;
	entityUid: Guid;
	uids: string[] | number[];
}

export class MetadataService extends Fetcher {

	load = async<TEntity>(viewId: string): Promise<DataView<TEntity>> => {
		return await this.post(Api.metadataView, { viewId: viewId });
	};

	fieldTypes = async (entityTypeCode: string): Promise<IFieldType[]> => {
		return this.post(Api.metadataFieldTypes, { entityTypeCode });
	};

	get = async (entityTypeCode: string, entityUid: Guid, uid: Guid): Promise<IDataField> => {
		return this.post(Api.metadataGet, { entityTypeCode, entityUid, uid });
	};

	insert = async (request: ManageFieldDataRequest): Promise<ApiResult> => {
		return this.post(Api.metadataInsert, request);
	};

	update = async (request: ManageFieldDataRequest): Promise<ApiResult> => {
		return this.post(Api.metadataUpdate, request);
	};

	delete = async (request: DeleteFieldDataRequest): Promise<ApiResult> => {
		return this.post(Api.metadataDelete, request);
	};
}
