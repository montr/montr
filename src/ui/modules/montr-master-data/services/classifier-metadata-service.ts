import { Fetcher } from "@montr-core/services";
import { DataView } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class ClassifierMetadataService extends Fetcher {

	load = async<TEntity>(typeCode: string): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(`${Constants.apiURL}/ClassifierMetadata/View`, { typeCode });

		return data;
	};

}
