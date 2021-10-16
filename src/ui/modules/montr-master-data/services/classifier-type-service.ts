import { ApiResult, DataResult, DataView, Guid, SearchRequest } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { ClassifierType } from "../models";
import { Api } from "../module";

interface GetClassifierType {
	typeCode?: string;
	uid?: Guid | string;
}

export class ClassifierTypeService extends Fetcher {

	metadata = async<TEntity>(typeCode: string, viewId: string): Promise<DataView<TEntity>> => {
		return this.post(Api.classifierTypeMetadata, { typeCode, viewId });
	};

	list = async (request?: SearchRequest): Promise<DataResult<ClassifierType>> => {
		return this.post(Api.classifierTypeList, request);
	};

	get = async (request: GetClassifierType): Promise<ClassifierType> => {
		return this.post(Api.classifierTypeGet, request);
	};

	create = async (): Promise<ClassifierType> => {
		return this.post(Api.classifierTypeCreate, {});
	};

	insert = async (item: ClassifierType): Promise<ApiResult> => {
		return this.post(Api.classifierTypeInsert, { item });
	};

	update = async (item: ClassifierType): Promise<ApiResult> => {
		return this.post(Api.classifierTypeUpdate, { item });
	};

	delete = async (uids: string[] | number[]): Promise<ApiResult> => {
		return this.post(Api.classifierTypeDelete, { uids });
	};
}
