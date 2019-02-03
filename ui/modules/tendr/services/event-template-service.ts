import { Fetcher } from "@montr-core/services";
import { Constants } from "../constants";
import { IEventTemplate } from "../models";

export class EventTemplateService extends Fetcher {

	list = async (): Promise<IEventTemplate[]> => {
		return this.post(`${Constants.baseURL}/EventTemplates/List`);
	};

}
