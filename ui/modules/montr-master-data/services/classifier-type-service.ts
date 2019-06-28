import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IDataResult, IApiResult } from "@montr-core/models";
import { IClassifierType } from "../models";

interface IGetClassifierType {
	typeCode?: string;
	uid?: Guid | string;
}

export class ClassifierTypeService extends Fetcher {
	list = async (companyUid: Guid): Promise<IDataResult<IClassifierType>> => {
		return this.post(`${Constants.baseURL}/classifierType/list`, { companyUid });
	};

	get = async (companyUid: Guid, request: IGetClassifierType): Promise<IClassifierType> => {
		return this.post(`${Constants.baseURL}/classifierType/get`, { companyUid, ...request });
	};

	insert = async (companyUid: Guid, data: IClassifierType): Promise<IInsertClassifierTypeResult> => {
		return this.post(`${Constants.baseURL}/classifierType/insert`, { companyUid, item: data });
	};

	update = async (companyUid: Guid, data: IClassifierType): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/classifierType/update`, { companyUid, item: data });
	};

	delete = async (companyUid: Guid, uids: string[] | number[]): Promise<number> => {
		return this.post(`${Constants.baseURL}/classifierType/delete`, { companyUid, uids });
	};
}

export interface IInsertClassifierTypeResult extends IApiResult {
	uid?: Guid;
}
