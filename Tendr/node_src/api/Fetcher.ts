import { message } from "antd";

function checkStatus(response: Response) {
    if (!response.ok) {
        message.error(`${response.status} (${response.statusText}) @ ${response.url}`);
        throw Error(`${response.status} (${response.statusText}) @ ${response.url}`);
    }

    return response;
}

const post = async (url: string, body?: any): Promise<any> => {

    const options: RequestInit = {
        method: "POST"
    };

    if (body) {
        options.headers = {
            'Content-Type': 'application/json'
        }
        options.body = JSON.stringify(body);
    }

    const response = await fetch(url, options);

    checkStatus(response)

    const data = await response.json();

    return data;
};

export const Fetcher = {
    post
};