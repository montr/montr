import { EventTemplate } from "./";
import { Constants } from "./Constants";

const fetchData = async (): Promise<EventTemplate[]> => {
    const url = `${Constants.baseURL}/EventTemplates`;

    const response = await fetch(url);
    const data = await (response.json());

    return data.map(mapToModel);
};

const mapToModel = (data: any): EventTemplate => {
    return {
        id: data.id,
        eventType: data.eventType,
        name: data.name,
        description: data.description,
    };
};

export const EventTemplateAPI = {
    fetchData
};