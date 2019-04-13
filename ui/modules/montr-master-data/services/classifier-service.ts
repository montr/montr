import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IDataResult } from "@montr-core/models";
import { IClassifier, IClassifierType } from "../models";

export class ClassifierService extends Fetcher {
	trees = async (companyUid: Guid, typeCode: string): Promise<IDataResult<IClassifierType>> => {
		return this.post(`${Constants.baseURL}/classifier/trees`, { companyUid, typeCode });
	};

	get = async (companyUid: Guid, typeCode: string, uid: Guid | string): Promise<IClassifier> => {
		return this.post(`${Constants.baseURL}/classifier/get`, { companyUid, typeCode, uid });
	};

	export = async (companyUid: Guid, request: any): Promise<IClassifier> => {
		return this.download(`${Constants.baseURL}/classifier/export`, { companyUid, ...request });
	};

	insert = async (companyUid: Guid, typeCode: string, data: IClassifier): Promise<Guid> => {
		return this.post(`${Constants.baseURL}/classifier/insert`, { companyUid, typeCode, item: data });
	};

	update = async (companyUid: Guid, data: IClassifier): Promise<number> => {
		return this.post(`${Constants.baseURL}/classifier/update`, { companyUid, item: data });
	};

	delete = async (companyUid: Guid, typeCode: string, uids: string[] | number[]): Promise<number> => {
		return this.post(`${Constants.baseURL}/classifier/delete`, { companyUid, typeCode, uids });
	};
}
