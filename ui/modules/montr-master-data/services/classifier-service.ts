import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { IClassifier } from "@montr-master-data/models";
import { Guid } from "@montr-core/models";

export class ClassifierService extends Fetcher {
	get = async (uid: Guid): Promise<IClassifier> => {
		return this.post(`${Constants.baseURL}/classifier/get`, { uid: uid });
	};

	insert = async (data: IClassifier): Promise<Guid> => {
		return this.post(`${Constants.baseURL}/classifier/insert`, data);
	};

	update = async (data: IClassifier): Promise<Guid> => {
		return this.post(`${Constants.baseURL}/classifier/update`, data);
	};
}
