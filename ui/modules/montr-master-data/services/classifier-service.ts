import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { IClassifier } from "@montr-master-data/models";
import { Guid } from "@montr-core/models";

export class ClassifierService extends Fetcher {
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
