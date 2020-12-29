import { Fetcher } from "@montr-core/services";
import { Guid, ApiResult, DataResult } from "@montr-core/models";
import { Classifier } from "../models";
import { Api } from "../module";

interface IClassifierSearchRequest {
	// todo: move to IPaging
	pageSize?: number;
	typeCode: string;
	focusUid?: Guid | string;
	searchTerm?: string;
}

export class ClassifierService extends Fetcher {

	list = async (request: IClassifierSearchRequest): Promise<DataResult<Classifier>> => {
		return this.post(Api.classifierList, request);
	};

	export = async (request: IClassifierSearchRequest): Promise<any> => {
		return this.download(Api.classifierExport, request);
	};

	create = async (typeCode: string, parentUid: Guid | string): Promise<Classifier> => {
		return this.post(Api.classifierCreate, { typeCode, parentUid });
	};

	get = async (typeCode: string, uid: Guid | string): Promise<Classifier> => {
		return this.post(Api.classifierGet, { typeCode, uid });
	};

	insert = async (typeCode: string, item: Classifier): Promise<ApiResult> => {
		return this.post(Api.classifierInsert, { item: { type: typeCode, ...item } });
	};

	update = async (typeCode: string, item: Classifier): Promise<ApiResult> => {
		return this.post(Api.classifierUpdate, { item: { type: typeCode, ...item } });
	};

	delete = async (typeCode: string, uids: string[] | number[]): Promise<number> => {
		return this.post(Api.classifierDelete, { typeCode, uids });
	};
}
