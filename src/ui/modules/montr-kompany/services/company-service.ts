import { ApiResult, DataResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { ICompany } from "../models";
import { Api } from "../module";

interface ICreateCompanyRequest {
	item: ICompany;
}

export class CompanyService extends Fetcher {

	list = async (): Promise<DataResult<ICompany>> => {
		return await this.post(Api.companyList);
	};

	create = async (request: ICreateCompanyRequest): Promise<ApiResult> => {
		return await this.post(Api.companyCreate, request);
	};
}
