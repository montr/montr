import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IApiResult } from "@montr-core/models";
import { IClassifier } from "../models";

interface IInsertClassifierRequest {
	typeCode: string;
	item: IClassifier;
}

export class ClassifierService extends Fetcher {

	get = async (companyUid: Guid, typeCode: string, uid: Guid | string): Promise<IClassifier> => {
		return this.post(`${Constants.apiURL}/classifier/get`, { companyUid, typeCode, uid });
	};

	export = async (companyUid: Guid, request: any): Promise<IClassifier> => {
		return this.download(`${Constants.apiURL}/classifier/export`, { companyUid, ...request });
	};

	insert = async (companyUid: Guid, request: IInsertClassifierRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifier/insert`, { companyUid, ...request });
	};

	update = async (companyUid: Guid, typeCode: string, item: IClassifier): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifier/update`, { companyUid, typeCode, item });
	};

	delete = async (companyUid: Guid, typeCode: string, uids: string[] | number[]): Promise<number> => {
		return this.post(`${Constants.apiURL}/classifier/delete`, { companyUid, typeCode, uids });
	};
}
