import { Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { ICompany } from "../models/";
import { Constants } from "./";

const list = async (): Promise<ICompany[]> => {
	return new Fetcher().post(
		`${Constants.apiURL}/Company/List`);
};

const create = async (item: ICompany): Promise<Guid> => {
	return new Fetcher().post(
		`${Constants.apiURL}/Company/Create`, item);
};

export const CompanyAPI = {
	list, create
};
