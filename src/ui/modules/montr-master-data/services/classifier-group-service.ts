import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, ApiResult, DataResult } from "@montr-core/models";
import { ClassifierGroup } from "../models";

interface IClassifierGroupSearchRequest {
	typeCode: string;
	treeCode?: string;
	treeUid?: Guid;
	parentUid?: Guid;
	focusUid?: Guid | string;
	expandSingleChild?: boolean;
}

export class ClassifierGroupService extends Fetcher {

	list = async (request: IClassifierGroupSearchRequest): Promise<DataResult<ClassifierGroup>> => {
		return this.post(`${Constants.apiURL}/classifierGroup/list`, request);
	};

	get = async (typeCode: string, treeUid: Guid, uid: Guid | string): Promise<ClassifierGroup> => {
		return this.post(`${Constants.apiURL}/classifierGroup/get`, { typeCode, treeUid, uid });
	};

	insert = async (typeCode: string, treeUid: Guid, item: ClassifierGroup): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierGroup/insert`, { typeCode, treeUid, item });
	};

	update = async (typeCode: string, item: ClassifierGroup): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierGroup/update`, { typeCode, item });
	};

	delete = async (typeCode: string, uid: Guid | string | number): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierGroup/delete`, { typeCode, uid });
	};
}
