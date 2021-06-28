import { ApiResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { IDocument } from "@montr-docs/models";
import { Api } from "../module";

export class CompanyRegistrationService extends Fetcher {

    listRequests = async (): Promise<IDocument[]> => {
        return await this.post(Api.companyRegistrationRequestList);
    };

    createRequest = async (): Promise<ApiResult> => {
        return await this.post(Api.companyRegistrationRequestCreate);
    };

}
