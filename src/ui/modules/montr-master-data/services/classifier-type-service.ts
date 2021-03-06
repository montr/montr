import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, DataResult, ApiResult } from "@montr-core/models";
import { ClassifierType } from "../models";
import { SearchRequest } from "@montr-core/models";

interface IGetClassifierType {
	typeCode?: string;
	uid?: Guid | string;
}

export class ClassifierTypeService extends Fetcher {
	list = async (request?: SearchRequest): Promise<DataResult<ClassifierType>> => {
		return this.post(`${Constants.apiURL}/classifierType/list`, request);
	};

	get = async (request: IGetClassifierType): Promise<ClassifierType> => {
		return this.post(`${Constants.apiURL}/classifierType/get`, request);
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
