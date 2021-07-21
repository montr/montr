import { Constants } from "@montr-core/.";
import { ApiResult, DataResult, DataView, Guid, SearchRequest } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { ClassifierType } from "../models";

interface GetClassifierType {
	typeCode?: string;
	uid?: Guid | string;
}

export class ClassifierTypeService extends Fetcher {

	metadata = async<TEntity>(typeCode: string, viewId: string): Promise<DataView<TEntity>> => {
		return this.post(`${Constants.apiURL}/classifierType/metadata`, { typeCode, viewId });
	};

	list = async (request?: SearchRequest): Promise<DataResult<ClassifierType>> => {
		return this.post(`${Constants.apiURL}/classifierType/list`, request);
	};

	get = async (request: GetClassifierType): Promise<ClassifierType> => {
		return this.post(`${Constants.apiURL}/classifierType/get`, request);
	};

	create = async (): Promise<ClassifierType> => {
		return this.post(`${Constants.apiURL}/classifierType/create`, {});
	};

	insert = async (item: ClassifierType): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierType/insert`, { item });
	};

	update = async (item: ClassifierType): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierType/update`, { item });
	};

	delete = async (uids: string[] | number[]): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierType/delete`, { uids });
	};
}
