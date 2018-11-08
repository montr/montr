import { Event } from "./";
import { Constants } from "./Constants";

function checkStatus(response: Response) {
    if (!response.ok) {
        throw Error(`${response.status} (${response.statusText}) @ ${response.url}`);
    }
    return response;
}

const load = async (): Promise<Event[]> => {

    const response = await fetch(
        `${Constants.baseURL}/Events/Load`, { method: "POST" });

    checkStatus(response)

    const data = await response.json();

    const models = data.map(mapToModel);
    
    return models;
};

const create = async (item: Event): Promise<boolean> => {

    const response = await fetch(
        `${Constants.baseURL}/Events/Create`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        });

    checkStatus(response)

    const data = await response.json();

    return data;
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
    load, create
};