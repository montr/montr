import { IEvent } from "./";
import { Constants } from "./Constants";
import { Fetcher } from "./Fetcher";

const getLoadUrl = (): string => {
    return `${Constants.baseURL}/Events/Load`;
}

const load = async (): Promise<IEvent[]> => {
    return Fetcher.post(getLoadUrl());
};

const create = async (configCode: string): Promise<number> => {
    return Fetcher.post(
        `${Constants.baseURL}/Events/Create`, {
            configCode: configCode
        });
};

export const EventAPI = {
    getLoadUrl, load, create
};