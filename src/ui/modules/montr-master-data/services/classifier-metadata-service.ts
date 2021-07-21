import { DataView } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

export class ClassifierMetadataService extends Fetcher {

	view = async<TEntity>(typeCode: string, viewId: string): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(Api.classifierMetadataView, { typeCode, viewId });

		return data;
	};

}
