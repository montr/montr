import { DataResult, LocaleString } from "../models";
import { Api } from "../module";
import { Fetcher } from "./fetcher";

interface ILocaleStringSearchRequest {
	locale: string;
	module: string;
}

export class LocaleStringService extends Fetcher {

	list = async (request: ILocaleStringSearchRequest): Promise<DataResult<LocaleString>> => {
		return this.post(Api.localeList, request);
	};

	export = async (request: ILocaleStringSearchRequest): Promise<any> => {
		return this.download(Api.localeExport, request);
	};
}
