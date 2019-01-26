import { Guid } from "@montr-core/.";
import { Fetcher } from "@montr-core/services";

import { ICompany } from "../models/company";
import { Constants } from "../Constants";

const list = async (): Promise<ICompany[]> => {
	return new Fetcher().post(
		`${Constants.baseURL}/Company/List`);
};

const create = async (item: ICompany): Promise<Guid> => {
	return new Fetcher().post(
		`${Constants.baseURL}/Company/Create`, item);
};

export const CompanyAPI = {
	list, create
};
