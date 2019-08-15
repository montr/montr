import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IApiResult, IDataResult } from "@montr-core/models";
import { IClassifierTree } from "../models";

interface IClassifierTreeSearchRequest {
	typeCode: string;
}

export class ClassifierTreeService extends Fetcher {

	list = async (companyUid: Guid, request: IClassifierTreeSearchRequest): Promise<IDataResult<IClassifierTree>> => {
		return this.post(`${Constants.apiURL}/classifierTree/list`, { companyUid, ...request });
	};

	get = async (companyUid: Guid, typeCode: string, uid: Guid | string): Promise<IClassifierTree> => {
		return this.post(`${Constants.apiURL}/classifierTree/get`, { companyUid, typeCode, uid });
	};

	insert = async (companyUid: Guid, typeCode: string, data: IClassifierTree): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierTree/insert`, { companyUid, typeCode, item: data });
	};

	update = async (companyUid: Guid, typeCode: string, data: IClassifierTree): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierTree/update`, { companyUid, typeCode, item: data });
	};

	delete = async (companyUid: Guid, typeCode: string, uids: Guid[]): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierTree/delete`, { companyUid, typeCode, uids });
	};
}
