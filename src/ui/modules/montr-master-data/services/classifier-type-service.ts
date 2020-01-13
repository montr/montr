import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IDataResult, IApiResult } from "@montr-core/models";
import { IClassifierType } from "../models";
import { ISearchRequest } from "@montr-core/models";

interface IGetClassifierType {
	typeCode?: string;
	uid?: Guid | string;
}

export class ClassifierTypeService extends Fetcher {
	list = async (request?: ISearchRequest): Promise<IDataResult<IClassifierType>> => {
		return this.post(`${Constants.apiURL}/classifierType/list`, request);
	};

	get = async (request: IGetClassifierType): Promise<IClassifierType> => {
		return this.post(`${Constants.apiURL}/classifierType/get`, request);
	};

	insert = async (item: IClassifierType): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierType/insert`, { item });
	};

	update = async (item: IClassifierType): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierType/update`, { item });
	};

	delete = async (uids: string[] | number[]): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierType/delete`, { uids });
	};
}
