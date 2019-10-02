import { Fetcher } from "@montr-core/services";
import { IDataView } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class ClassifierMetadataService extends Fetcher {

	load = async<TEntity>(typeCode: string): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.apiURL}/ClassifierMetadata/View`, { typeCode });

		return data;
	}

}
