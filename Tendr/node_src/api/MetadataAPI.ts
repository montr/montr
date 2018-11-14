import { IDataView } from "./";
import { Constants } from "./Constants";
import { Fetcher } from "./Fetcher";

const getLoadUrl = (): string => {
    return `${Constants.baseURL}/Metadata/View`;
}

const load = async (viewId: string): Promise<IDataView> => {
    return Fetcher.post(getLoadUrl(), { viewId: viewId });
};

export const MetadataAPI = {
    getLoadUrl, load
};