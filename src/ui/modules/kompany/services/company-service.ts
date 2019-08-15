import { Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { ICompany } from "../models";

export class CompanyService extends Fetcher {

	list = async (): Promise<ICompany[]> => {
		return await this.post(`${Constants.apiURL}/Company/List`);
	};

	create = async (item: ICompany): Promise<Guid> => {
		return await this.post(`${Constants.apiURL}/Company/Create`, item);
	};
}
