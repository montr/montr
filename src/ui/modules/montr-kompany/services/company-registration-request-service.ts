import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { IDocument } from "@montr-docs/models";
import { Api } from "../module";

export class CompanyRegistrationRequestService extends Fetcher {

    search = async (): Promise<IDocument[]> => {
        return await this.post(Api.companyRegistrationRequestList);
    };

    create = async (): Promise<ApiResult> => {
        return await this.post(Api.companyRegistrationRequestCreate);
    };

    delete = async (uid: string | Guid): Promise<ApiResult> => {
        return await this.post(Api.companyRegistrationRequestDelete, { uid });
    };

}
