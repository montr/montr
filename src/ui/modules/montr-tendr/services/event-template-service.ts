import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { DataResult } from "@montr-core/models";
import { IEvent } from "../models";

export class EventTemplateService extends Fetcher {

	list = async (): Promise<DataResult<IEvent>> => {
		return this.post(`${Constants.apiURL}/EventTemplate/List`, {});
	};

}
