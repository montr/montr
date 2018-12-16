import { Fetcher } from "montr$core/api/";

import { IEvent } from "./";
import { Constants } from "./Constants";
import { IApiResult } from "./IApiResult";

const getLoadUrl = (): string => {
    return `${Constants.baseURL}/Events/Load`;
}

const load = async (): Promise<IEvent[]> => {
    return Fetcher.post(getLoadUrl());
};

const get = async (id: number): Promise<IEvent> => {
    return Fetcher.post(
        `${Constants.baseURL}/Events/Get`, { id: id });
};

const create = async (configCode: string): Promise<number> => {
    return Fetcher.post(
        `${Constants.baseURL}/Events/Create`, { configCode: configCode });
};

const update = async (data: IEvent): Promise<IApiResult> => {
    return Fetcher.post(
        `${Constants.baseURL}/Events/Update`, data);
};

const publish = async (id: number): Promise<IApiResult> => {
    return Fetcher.post(
        `${Constants.baseURL}/Events/Publish`, { id: id });
};

const cancel = async (id: number): Promise<IApiResult> => {
    return Fetcher.post(
        `${Constants.baseURL}/Events/Cancel`, { id: id });
};

export const EventAPI = {
    getLoadUrl, load, get, create, update, publish, cancel
};