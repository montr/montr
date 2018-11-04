import { Event } from "./";
import { Constants } from "./Constants";

const fetchData = async (): Promise<Event[]> => {
    const url = `${Constants.baseURL}/Events`;

    const response = await fetch(url);
    const data = await (response.json());

    return data.map(mapToModel);
};

const mapToModel = (data: any): Event => {
    return {
        id: data.id,
        eventType: data.eventType,
        number: data.number,
        name: data.name,
        description: data.description,
    };
};

export const EventAPI = {
    fetchData
};