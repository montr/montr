import { Fetcher } from "montr$core/api/";

import { Constants } from "./Constants";
import { IApiResult } from "./IApiResult";
import { IAccountInfo } from "./IAccountInfo";

const info = async (): Promise<IAccountInfo> => {
    return Fetcher.post(
        `${Constants.baseURL}/Account/Info`);
};

const logout = async (): Promise<IApiResult> => {
    return Fetcher.post(
        `${Constants.baseURL}/Account/Logout`);
};

export const AccountAPI = {
    info, logout
};