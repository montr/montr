import { Fetcher } from "@montr-core/services";
import { Guid, IApiResult, IDataResult } from "@montr-core/models";
import { IClassifier } from "../models";
import { Api } from "../module";

interface IClassifierSearchRequest {
	// todo: move to IPaging
	pageSize?: number;
	typeCode: string;
	focusUid?: Guid | string;
	searchTerm?: string;
}

interface IInsertClassifierRequest {
	typeCode: string;
	item: IClassifier;
}

export class ClassifierService extends Fetcher {

	list = async (request: IClassifierSearchRequest): Promise<IDataResult<IClassifier>> => {
		return this.post(Api.classifierList, request);
	};

	export = async (request: IClassifierSearchRequest): Promise<any> => {
		return this.download(Api.classifierExport, request);
	};

	create = async (typeCode: string, parentUid: Guid | string): Promise<IClassifier> => {
		return this.post(Api.classifierCreate, { typeCode, parentUid });
	};

	get = async (typeCode: string, uid: Guid | string): Promise<IClassifier> => {
		return this.post(Api.classifierGet, { typeCode, uid });
	};

	insert = async (request: IInsertClassifierRequest): Promise<IApiResult> => {
		return this.post(Api.classifierInsert, request);
	};

	update = async (typeCode: string, item: IClassifier): Promise<IApiResult> => {
		return this.post(Api.classifierUpdate, { typeCode, item });
	};

	delete = async (typeCode: string, uids: string[] | number[]): Promise<number> => {
		return this.post(Api.classifierDelete, { typeCode, uids });
	};
}
