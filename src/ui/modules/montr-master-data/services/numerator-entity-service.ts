import { ApiResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { NumeratorEntity } from "../models";
import { Api } from "../module";

export class NumeratorEntityService extends Fetcher {

    save = async (item: NumeratorEntity): Promise<ApiResult> => {
        return this.post(Api.numeratorEntitySave, { item });
    };

}
