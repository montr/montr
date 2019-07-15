import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IApiResult, IDataResult } from "@montr-core/models";
import { IClassifierGroup } from "../models";

interface IClassifierGroupSearchRequest {
	typeCode: string;
	treeUid: Guid;
	parentUid?: Guid
	focusUid?: Guid | string;
	expandSingleChild?: boolean;
}

export class ClassifierGroupService extends Fetcher {

	list = async (companyUid: Guid, request: IClassifierGroupSearchRequest): Promise<IDataResult<IClassifierGroup>> => {
		return this.post(`${Constants.baseURL}/classifierGroup/list`, { companyUid, ...request });
	};

	get = async (companyUid: Guid, typeCode: string, treeUid: Guid, uid: Guid | string): Promise<IClassifierGroup> => {
		return this.post(`${Constants.baseURL}/classifierGroup/get`, { companyUid, typeCode, treeUid, uid });
	};

	insert = async (companyUid: Guid, typeCode: string, treeUid: Guid, data: IClassifierGroup): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/classifierGroup/insert`, { companyUid, typeCode, treeUid, item: data });
	};

	update = async (companyUid: Guid, typeCode: string, data: IClassifierGroup): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/classifierGroup/update`, { companyUid, typeCode, item: data });
	};

	delete = async (companyUid: Guid, typeCode: string, uid: Guid | string | number): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/classifierGroup/delete`, { companyUid, typeCode, uid });
	};
}
