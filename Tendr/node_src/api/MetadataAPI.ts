import { DataColumn } from "./";
import { Constants } from "./Constants";
import { Fetcher } from "./Fetcher";

const getLoadUrl = (): string => {
    return `${Constants.baseURL}/Metadata/Columns`;
}

const load = async (viewId: string): Promise<DataColumn[]> => {
    return Fetcher.post(getLoadUrl(), { viewId: viewId });
};

export const MetadataAPI = {
    getLoadUrl, load
};