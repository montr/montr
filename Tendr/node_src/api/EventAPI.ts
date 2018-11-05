import { Event } from "./";
import { Constants } from "./Constants";

function checkStatus(response: Response) {
    if (!response.ok) {
        throw Error(`${response.status} (${response.statusText}) @ ${response.url}`);
    }
    return response;
}

const fetchData = async (): Promise<Event[]> => {

    const response = await fetch(
        `${Constants.baseURL}/Events/Load`, { method: "POST" });

    checkStatus(response)

    const data = await response.json();

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