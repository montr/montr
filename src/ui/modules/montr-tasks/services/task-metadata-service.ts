import { DataView } from "@montr-core/models/data-view";
import { Fetcher } from "@montr-core/services/fetcher";
import { Api } from "@montr-tasks/module";

export class TaskMetadataService extends Fetcher {
    searchMetadata = async<TEntity>(): Promise<DataView<TEntity>> => {
        return await this.post(Api.taskSearchMetadata, {});
    };
}
