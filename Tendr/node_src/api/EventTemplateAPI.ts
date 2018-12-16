import { Fetcher } from "montr$core/api/";

import { IEventTemplate } from "./";
import { Constants } from "./Constants";

const load = async (): Promise<IEventTemplate[]> => {
    return Fetcher.post(`${Constants.baseURL}/EventTemplates/Load`);
};

export const EventTemplateAPI = {
    load
};