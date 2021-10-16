import { DataView } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

export class CompanyMetadataService extends Fetcher {

	load = async<TEntity>(): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(Api.companyMetadataView, {});

		return data;
	};

}
