import { ApiResult, DataResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "@montr-master-data/module";
import { ClassifierTree } from "../models";

interface IClassifierTreeSearchRequest {
	typeCode: string;
}

export class ClassifierTreeService extends Fetcher {

	list = async (request: IClassifierTreeSearchRequest): Promise<DataResult<ClassifierTree>> => {
		return this.post(Api.classifierTreeList, request);
	};

	get = async (typeCode: string, uid: Guid | string): Promise<ClassifierTree> => {
		return this.post(Api.classifierTreeGet, { typeCode, uid });
	};

	insert = async (typeCode: string, data: ClassifierTree): Promise<ApiResult> => {
		return this.post(Api.classifierTreeInsert, { typeCode, item: data });
	};

	update = async (typeCode: string, data: ClassifierTree): Promise<ApiResult> => {
		return this.post(Api.classifierTreeUpdate, { typeCode, item: data });
	};

	delete = async (typeCode: string, uids: Guid[]): Promise<ApiResult> => {
		return this.post(Api.classifierTreeDelete, { typeCode, uids });
	};
}
