import { Fetcher } from "@montr-core/services";
import { IEventTemplate } from "./";
import { Constants } from "../constants";

const load = async (): Promise<IEventTemplate[]> => {
	return new Fetcher().post(`${Constants.baseURL}/EventTemplates/Load`);
};

export const EventTemplateAPI = {
	load
};
