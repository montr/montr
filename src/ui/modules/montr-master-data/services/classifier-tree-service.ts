import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, ApiResult, DataResult } from "@montr-core/models";
import { IClassifierTree } from "../models";

interface IClassifierTreeSearchRequest {
	typeCode: string;
}

export class ClassifierTreeService extends Fetcher {

	list = async (request: IClassifierTreeSearchRequest): Promise<DataResult<IClassifierTree>> => {
		return this.post(`${Constants.apiURL}/classifierTree/list`, request);
	};

	get = async (typeCode: string, uid: Guid | string): Promise<IClassifierTree> => {
		return this.post(`${Constants.apiURL}/classifierTree/get`, { typeCode, uid });
	};

	insert = async (typeCode: string, data: IClassifierTree): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierTree/insert`, { typeCode, item: data });
	};

	update = async (typeCode: string, data: IClassifierTree): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierTree/update`, { typeCode, item: data });
	};

	delete = async (typeCode: string, uids: Guid[]): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierTree/delete`, { typeCode, uids });
	};
}
