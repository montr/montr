import { Fetcher, Guid } from "@montr-core/.";

import { ICompany } from "./ICompany";
import { Constants } from "./Constants";

const create = async (item: ICompany): Promise<Guid> => {
    return Fetcher.post(
        `${Constants.baseURL}/Company/Create`, item);
};

export const CompanyAPI = {
    create
};