import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid } from "@montr-core/models";
import { IClassifierGroup } from "../models";

interface ClassifierGroupSearchRequest {
	typeCode: string;
	treeCode: string;
	parentUid?: Guid
	focusUid?: Guid | string;
}

export class ClassifierGroupService extends Fetcher {
	list = async (companyUid: Guid, request: ClassifierGroupSearchRequest): Promise<IClassifierGroup[]> => {
		return this.post(`${Constants.baseURL}/classifierGroup/list`, { companyUid, ...request });
	};

	get = async (companyUid: Guid, typeCode: string, treeCode: string, uid: Guid | string): Promise<IClassifierGroup> => {
		return this.post(`${Constants.baseURL}/classifierGroup/get`, { companyUid, typeCode, treeCode, uid });
	};

	insert = async (companyUid: Guid, typeCode: string, treeCode: string, data: IClassifierGroup): Promise<Guid> => {
		return this.post(`${Constants.baseURL}/classifierGroup/insert`, { companyUid, typeCode, treeCode, item: data });
	};

	update = async (companyUid: Guid, data: IClassifierGroup): Promise<number> => {
		return this.post(`${Constants.baseURL}/classifierGroup/update`, { companyUid, item: data });
	};

	delete = async (companyUid: Guid, typeCode: string, treeCode: string, uid: Guid | string | number): Promise<number> => {
		return this.post(`${Constants.baseURL}/classifierGroup/delete`, { companyUid, typeCode, treeCode, uid });
	};
}
