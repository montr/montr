import { Fetcher } from ".";
import { Constants } from "..";
import { IMenu } from "../models";

export class ContentService extends Fetcher {

	getMenu = async (menuId: string): Promise<IMenu> => {
		return await this.post(`${Constants.apiURL}/Content/Menu`, { menuId });
	}

}
