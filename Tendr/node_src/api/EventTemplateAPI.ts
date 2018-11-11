import { IEventTemplate } from "./";
import { Constants } from "./Constants";
import { Fetcher } from "./Fetcher";

const load = async (): Promise<IEventTemplate[]> => {
    return Fetcher.post(`${Constants.baseURL}/EventTemplates/Load`);
};

export const EventTemplateAPI = {
    load
};