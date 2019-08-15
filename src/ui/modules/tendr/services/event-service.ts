import { Fetcher } from "@montr-core/services";
import { IApiResult } from "@montr-core/models/api-result";
import { Constants } from "@montr-core/.";
import { IEvent } from "../models";

export class EventService extends Fetcher {

	// todo: remove, use list method in data grid
	getLoadUrl = (): string => {
		return `${Constants.apiURL}/Events/List`;
	}

	list = async (): Promise<IEvent[]> => {
		return this.post(`${Constants.apiURL}/Events/List`);
	};

	get = async (id: number): Promise<IEvent> => {
		return this.post(`${Constants.apiURL}/Events/Get`, { id: id });
	};

	create = async (data: IEvent): Promise<number> => {
		return this.post(`${Constants.apiURL}/Events/Create`, data);
	};

	update = async (data: IEvent): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/Events/Update`, data);
	};

	publish = async (id: number): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/Events/Publish`, { id: id });
	};

	cancel = async (id: number): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/Events/Cancel`, { id: id });
	};

}
