import { Fetcher } from "@montr-core/services";
import { IDataView, Guid } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class ClassifierMetadataService extends Fetcher {

	load = async<TEntity>(companyUid: Guid, typeCode: string): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.apiURL}/ClassifierMetadata/View`, { companyUid, typeCode });

		return data;
	}

}
