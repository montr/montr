import { ApiResult, DataResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { ClassifierGroup } from "../models";
import { Api } from "../module";

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
		return this.post(Api.classifierGroupList, request);
	};

	get = async (typeCode: string, treeUid: Guid, uid: Guid | string): Promise<ClassifierGroup> => {
		return this.post(Api.classifierGroupGet, { typeCode, treeUid, uid });
	};

	insert = async (typeCode: string, treeUid: Guid, item: ClassifierGroup): Promise<ApiResult> => {
		return this.post(Api.classifierGroupInsert, { typeCode, treeUid, item });
	};

	update = async (typeCode: string, item: ClassifierGroup): Promise<ApiResult> => {
		return this.post(Api.classifierGroupUpdate, { typeCode, item });
	};

	delete = async (typeCode: string, uid: Guid | string | number): Promise<ApiResult> => {
		return this.post(Api.classifierGroupDelete, { typeCode, uid });
	};
}
