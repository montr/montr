import { Fetcher } from "./fetcher";
import { Constants } from "..";
import { LocaleString, DataResult } from "../models";

interface ILocaleStringSearchRequest {
	locale: string;
	module: string;
}

export class LocaleStringService extends Fetcher {

	list = async (request: ILocaleStringSearchRequest): Promise<DataResult<LocaleString>> => {
		return this.post(`${Constants.apiURL}/locale/list`, request);
	};

	export = async (request: ILocaleStringSearchRequest): Promise<any> => {
		return this.download(`${Constants.apiURL}/locale/export`, request);
	};
}
