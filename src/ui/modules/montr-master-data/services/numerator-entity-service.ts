import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { NumeratorEntity } from "../models";
import { Api } from "../module";

export class NumeratorEntityService extends Fetcher {

    get = async (entityTypeCode: string, entityUid: Guid): Promise<NumeratorEntity> => {
        return this.post(Api.numeratorEntityGet, { entityTypeCode, entityUid });
    };

    save = async (item: NumeratorEntity): Promise<ApiResult> => {
        return this.post(Api.numeratorEntitySave, { item });
    };

}
