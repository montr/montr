import { Fetcher } from "@montr-core/services";
import { Api } from "@montr-master-data/module";
import { Guid } from "@montr-core/models";
import { INumerator } from "@montr-master-data/models/numerator";

export class NumeratorService extends Fetcher {

	create = async (): Promise<INumerator> => {
		return this.post(Api.numeratorCreate, {});
	};

	get = async (uid: Guid | string): Promise<INumerator> => {
		return this.post(Api.numeratorGet, { uid });
	};
}
