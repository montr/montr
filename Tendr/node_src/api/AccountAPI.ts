import { Constants } from "./Constants";
import { Fetcher } from "./Fetcher";
import { IApiResult } from "./IApiResult";

const logout = async (): Promise<IApiResult> => {
    return Fetcher.post(
        `${Constants.baseURL}/Account/Logout`);
};

export const AccountAPI = {
    logout
};