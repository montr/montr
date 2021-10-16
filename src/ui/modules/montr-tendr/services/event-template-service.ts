import { DataResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { IEvent } from "../models";
import { Api } from "../module";

export class EventTemplateService extends Fetcher {

	list = async (): Promise<DataResult<IEvent>> => {
		return this.post(Api.eventTemplateList, {});
	};

}
