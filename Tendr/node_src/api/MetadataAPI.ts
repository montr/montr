import { DataColumn } from "./";
import { Constants } from "./Constants";

function checkStatus(response: Response) {
    if (!response.ok) {
        throw Error(`${response.status} (${response.statusText}) @ ${response.url}`);
    }
    return response;
}

const fetchData = async (viewId: string): Promise<DataColumn[]> => {
    const response = await fetch(
        `${Constants.baseURL}/Metadata/Columns`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ viewId: viewId })
        });

    checkStatus(response)

    const data = await response.json();

    return data.map((item: any): DataColumn => {
        return {
            key: item.key,
            path: item.path,
            name: item.name,
            align: item.align,
            sortable: item.sortable,
            width: item.width,
        };
    });
};

export const MetadataAPI = {
    fetchData
};