import { Fetcher } from "@montr-core/services";
import { IApiResult } from "@montr-core/models/api-result";
import { Constants } from "../constants";
import { IEvent } from "../models";

export class EventService extends Fetcher {

	// todo: remove, use list method in data grid
	getLoadUrl = (): string => {
		return `${Constants.baseURL}/Events/List`;
	}

	list = async (): Promise<IEvent[]> => {
		return this.post(`${Constants.baseURL}/Events/List`);
	};

	get = async (id: number): Promise<IEvent> => {
		return this.post(`${Constants.baseURL}/Events/Get`, { id: id });
	};

	create = async (data: IEvent): Promise<number> => {
		return this.post(`${Constants.baseURL}/Events/Create`, data);
	};

	update = async (data: IEvent): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/Events/Update`, data);
	};

	publish = async (id: number): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/Events/Publish`, { id: id });
	};

	cancel = async (id: number): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/Events/Cancel`, { id: id });
	};

}
