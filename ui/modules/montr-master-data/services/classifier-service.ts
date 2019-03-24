import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IDataResult } from "@montr-core/models";
import { IClassifier, IClassifierType, IClassifierGroup } from "../models";

export class ClassifierService extends Fetcher {
	types = async (companyUid: Guid): Promise<IDataResult<IClassifierType>> => {
		return this.post(`${Constants.baseURL}/classifier/types`, { companyUid });
	};

	trees = async (companyUid: Guid, typeCode: string): Promise<IDataResult<IClassifierType>> => {
		return this.post(`${Constants.baseURL}/classifier/trees`, { companyUid, typeCode });
	};

	groups = async (companyUid: Guid, typeCode: string, treeCode: string, parentCode: string): Promise<IClassifierGroup[]> => {
		return this.post(`${Constants.baseURL}/classifier/groups`, { companyUid, typeCode, treeCode, parentCode });
	};

	get = async (uid: Guid): Promise<IClassifier> => {
		return this.post(`${Constants.baseURL}/classifier/get`, { uid: uid });
	};

	export = async (companyUid: Guid, request: any): Promise<IClassifier> => {
		return this.download(`${Constants.baseURL}/classifier/export`, { companyUid, ...request });
	};

	insert = async (companyUid: Guid, data: IClassifier): Promise<Guid> => {
		return this.post(`${Constants.baseURL}/classifier/insert`, { companyUid, item: data });
	};

	update = async (companyUid: Guid, data: IClassifier): Promise<Guid> => {
		return this.post(`${Constants.baseURL}/classifier/update`, { companyUid, item: data });
	};

	delete = async (companyUid: Guid, uids: string[] | number[]): Promise<number> => {
		return this.post(`${Constants.baseURL}/classifier/delete`, { companyUid, uids });
	};
}
