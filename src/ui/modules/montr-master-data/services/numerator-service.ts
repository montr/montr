import { Fetcher } from "@montr-core/services";
import { Api } from "@montr-master-data/module";
import { Guid, IApiResult } from "@montr-core/models";
import { INumerator } from "@montr-master-data/models";

interface IInsertNumeratorRequest {
	item: INumerator;
}

export class NumeratorService extends Fetcher {

	create = async (): Promise<INumerator> => {
		return this.post(Api.numeratorCreate, {});
	};

	get = async (uid: Guid | string): Promise<INumerator> => {
		return this.post(Api.numeratorGet, { uid });
	};

	insert = async (request: IInsertNumeratorRequest): Promise<IApiResult> => {
		return this.post(Api.numeratorInsert, request);
	};

	update = async (item: INumerator): Promise<IApiResult> => {
		return this.post(Api.numeratorUpdate, { item });
	};

	delete = async (uids: string[] | number[]): Promise<IApiResult> => {
		return this.post(Api.numeratorDelete, { uids });
	};
}
