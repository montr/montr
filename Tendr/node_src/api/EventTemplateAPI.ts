import { EventTemplate } from "./";
import { Constants } from "./Constants";

const fetchData = async (): Promise<EventTemplate[]> => {

    const response = await fetch(
        `${Constants.baseURL}/EventTemplates/Load`, { method: "POST" });

    const data = await response.json();

    return data;
};

export const EventTemplateAPI = {
    fetchData
};