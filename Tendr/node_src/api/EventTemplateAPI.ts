import { EventTemplate } from "./";
import { Constants } from "./Constants";

const fetchData = async (): Promise<EventTemplate[]> => {

    const response = await fetch(
        `${Constants.baseURL}/EventTemplates/Load`, { method: "POST" });

    const data = await response.json();

    return data.map(mapToModel);
};

const mapToModel = (data: any): EventTemplate => {
    return {
        configCode: data.configCode,
        name: data.name,
        description: data.description,
    };
};

export const EventTemplateAPI = {
    fetchData
};