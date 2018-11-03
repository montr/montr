import { EventTemplate } from "./EventTemplate";

const baseURL = "http://localhost:5000/api";

const fetchEventTemplates = async (): Promise<EventTemplate[]> => {
    const url = `${baseURL}/EventTemplates`;

    const response = await fetch(url);
    const data = await (response.json());

    return mapToModels(data);
};

const mapToModels = (data: any[]): EventTemplate[] => {
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

export const API = {
    fetchEventTemplates
};