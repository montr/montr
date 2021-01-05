import { ApiResult, IIndexer } from "../models";
import { Fetcher } from ".";
import { Api } from "../module";

export class SetupService extends Fetcher {

    save = async (request: IIndexer): Promise<ApiResult> => {
        return this.post(Api.setupSave, request);
    };

}
