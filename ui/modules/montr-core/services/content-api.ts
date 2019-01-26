import { Fetcher } from ".";
import { Constants } from "../Constants";
import { IMenu } from "@montr-core/models";

const getMenu = async (menuId: string): Promise<IMenu> => {
	const data: IMenu = await new Fetcher().post(
		`${Constants.baseURL}/Content/Menu`, { menuId: menuId });

	return data;
};

export const ContentAPI = {
	getMenu
};
