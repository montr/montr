import { DataResult, Guid } from "@montr-core/models";
import { ApiResult } from "@montr-core/models/api-result";
import { Fetcher } from "@montr-core/services";
import { IEvent } from "../models";
import { Api } from "../module";

export class EventService extends Fetcher {

	list = async (): Promise<DataResult<IEvent>> => {
		return this.post(Api.eventsList);
	};

	get = async (uid: Guid | string): Promise<IEvent> => {
		return this.post(Api.eventsGet, { uid });
	};

	insert = async (item: IEvent): Promise<ApiResult> => {
		return this.post(Api.eventsInsert, { item });
	};

	update = async (item: IEvent): Promise<ApiResult> => {
		return this.post(Api.eventsUpdate, { item });
	};

	publish = async (uid: number | string): Promise<ApiResult> => {
		return this.post(Api.eventsPublish, { uid });
	};

	cancel = async (uid: number | string): Promise<ApiResult> => {
		return this.post(Api.eventsCancel, { uid });
	};

}
