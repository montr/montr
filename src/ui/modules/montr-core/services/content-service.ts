import { Fetcher } from "./fetcher";
import { IMenu } from "../models";
import { Api } from "../module";

export class ContentService extends Fetcher {

	getMenu = async (menuId: string): Promise<IMenu> => {
		return await this.post(Api.contentMenu, { menuId });
	};

}
