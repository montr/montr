import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IApiResult, IDataResult } from "@montr-core/models";
import { IClassifier } from "../models";

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
		return this.post(`${Constants.apiURL}/classifier/list`, request);
	};

	get = async (typeCode: string, uid: Guid | string): Promise<IClassifier> => {
		return this.post(`${Constants.apiURL}/classifier/get`, { typeCode, uid });
	};

	export = async (request: IClassifierSearchRequest): Promise<any> => {
		return this.download(`${Constants.apiURL}/classifier/export`, request);
	};

	insert = async (request: IInsertClassifierRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifier/insert`, request);
	};

	update = async (typeCode: string, item: IClassifier): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifier/update`, { typeCode, item });
	};

	delete = async (typeCode: string, uids: string[] | number[]): Promise<number> => {
		return this.post(`${Constants.apiURL}/classifier/delete`, { typeCode, uids });
	};
}
