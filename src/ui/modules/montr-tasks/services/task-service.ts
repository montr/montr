import { DataView, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Task } from "../models";
import { Api } from "../module";

export class TaskService extends Fetcher {
	metadata = async<TEntity>(viewId: string, taskUid?: Guid | string): Promise<DataView<TEntity>> => {
		return await this.post(Api.taskMetadata, { viewId, taskUid });
	};

	get = async (uid: Guid | string): Promise<Task> => {
		return this.post(Api.taskGet, { uid });
	};
}
