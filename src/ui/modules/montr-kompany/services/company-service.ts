import { ApiResult, DataResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Company } from "../models";
import { Api } from "../module";

interface CreateCompanyRequest {
	item: Company;
}

export class CompanyService extends Fetcher {

	list = async (): Promise<DataResult<Company>> => {
		return await this.post(Api.companyList);
	};

	create = async (request: CreateCompanyRequest): Promise<ApiResult> => {
		return await this.post(Api.companyCreate, request);
	};
}
