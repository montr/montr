import { Fetcher } from ".";
import { IMenu } from "../models";
import { Constants } from "..";

export class ContentService extends Fetcher {

	getMenu = async (menuId: string): Promise<IMenu> => {
		return await this.post(`${Constants.apiURL}/Content/Menu`, { menuId: menuId });
	}

}
