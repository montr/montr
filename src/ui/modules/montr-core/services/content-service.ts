import { IMenu } from "../models";
import { Api } from "../module";
import { Fetcher } from "./fetcher";

export class ContentService extends Fetcher {

	getMenu = async (menuId: string): Promise<IMenu> => {
		return await this.post(Api.contentMenu, { menuId });
	};

}
