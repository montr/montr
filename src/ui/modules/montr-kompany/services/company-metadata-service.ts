import { Fetcher } from "@montr-core/services";
import { IDataView } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class CompanyMetadataService extends Fetcher {

	load = async<TEntity>(): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.apiURL}/companyMetadata/View`, {});

		return data;
	};

}
