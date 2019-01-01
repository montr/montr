import { Guid } from "@montr-core/.";
import { Fetcher } from "@montr-core/services";

import { ICompany } from "./ICompany";
import { Constants } from "./Constants";

const create = async (item: ICompany): Promise<Guid> => {
    return new Fetcher().post(
        `${Constants.baseURL}/Company/Create`, item);
};

export const CompanyAPI = {
    create
};
