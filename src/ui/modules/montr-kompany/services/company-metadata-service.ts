import { Fetcher } from "@montr-core/services";
import { DataView } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class CompanyMetadataService extends Fetcher {

	load = async<TEntity>(): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(`${Constants.apiURL}/companyMetadata/View`, {});

		return data;
	};

}
