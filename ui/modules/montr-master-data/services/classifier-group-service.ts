import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IApiResult, IDataResult } from "@montr-core/models";
import { IClassifierGroup } from "../models";

interface ClassifierGroupSearchRequest {
	typeCode: string;
	// treeCode: string;
	parentUid?: Guid
	focusUid?: Guid | string;
	expandSingleChild?: boolean;
}

export class ClassifierGroupService extends Fetcher {
	list = async (companyUid: Guid, request: ClassifierGroupSearchRequest): Promise<IDataResult<IClassifierGroup>> => {
		return this.post(`${Constants.baseURL}/classifierGroup/list`, { companyUid, ...request });
	};

	get = async (companyUid: Guid, typeCode: string, treeCode: string, uid: Guid | string): Promise<IClassifierGroup> => {
		return this.post(`${Constants.baseURL}/classifierGroup/get`, { companyUid, typeCode, treeCode, uid });
	};

	insert = async (companyUid: Guid, typeCode: string, treeCode: string, data: IClassifierGroup): Promise<IInsertClassifierGroupResult> => {
		return this.post(`${Constants.baseURL}/classifierGroup/insert`, { companyUid, typeCode, treeCode, item: data });
	};

	update = async (companyUid: Guid, typeCode: string, treeCode: string, data: IClassifierGroup): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/classifierGroup/update`, { companyUid, typeCode, treeCode, item: data });
	};

	delete = async (companyUid: Guid, typeCode: string, treeCode: string, uid: Guid | string | number): Promise<number> => {
		return this.post(`${Constants.baseURL}/classifierGroup/delete`, { companyUid, typeCode, treeCode, uid });
	};
}

export interface IInsertClassifierGroupResult extends IApiResult {
	uid?: Guid;
}
