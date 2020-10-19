import { Fetcher } from "@montr-core/services";
import { Guid, DataResult } from "@montr-core/models";
import { ApiResult } from "@montr-core/models/api-result";
import { Constants } from "@montr-core/.";
import { IEvent } from "../models";

export class EventService extends Fetcher {

	// todo: remove, use list method in data grid
	getLoadUrl = (): string => {
		return `${Constants.apiURL}/Events/List`;
	}

	list = async (): Promise<DataResult<IEvent>> => {
		return this.post(`${Constants.apiURL}/Events/List`);
	};

	get = async (uid: Guid | string): Promise<IEvent> => {
		return this.post(`${Constants.apiURL}/Events/Get`, { uid });
	};

	insert = async (item: IEvent): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/Events/Insert`, { item });
	};

	update = async (item: IEvent): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/Events/Update`, { item });
	};

	publish = async (uid: number | string): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/Events/Publish`, { uid });
	};

	cancel = async (uid: number | string): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/Events/Cancel`, { uid });
	};

}
