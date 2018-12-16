import { Fetcher, Guid } from "montr$core/api/";

import { ICompany } from "./";
import { Constants } from "./Constants";

const create = async (item: ICompany): Promise<Guid> => {
    return Fetcher.post(
        `${Constants.baseURL}/Company/Create`, item);
};

export const CompanyAPI = {
    create
};