import { Event } from "./";
import { Constants } from "./Constants";

import { message } from 'antd';

function checkStatus(response: Response) {
    if (!response.ok) {
        message.error(`${response.status} (${response.statusText}) @ ${response.url}`);
        throw Error(`${response.status} (${response.statusText}) @ ${response.url}`);
    }

    return response;
}

const load = async (): Promise<Event[]> => {

    const response = await fetch(
        `${Constants.baseURL}/Events/Load`, { method: "POST" });

    checkStatus(response)

    const data = await response.json();

    return data;
};

const create = async (configCode: string): Promise<number> => {

    const response = await fetch(
        `${Constants.baseURL}/Events/Create`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                configCode: configCode
            })
        });

    checkStatus(response)

    const data = await response.json();

    return data;
};

export const EventAPI = {
    load, create
};
