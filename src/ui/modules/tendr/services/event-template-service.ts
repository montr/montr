import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { IDataResult } from "@montr-core/models";
import { IEvent } from "../models";

export class EventTemplateService extends Fetcher {

	list = async (): Promise<IDataResult<IEvent>> => {
		return this.post(`${Constants.apiURL}/EventTemplate/List`, {});
	};

}
