import { Fetcher } from "../";

import { Constants, IMenu } from "./";

const getMenu = async<TEntity>(menuId: string): Promise<IMenu> => {
	const data: IMenu = await Fetcher.post(
		`${Constants.baseURL}/Content/Menu`, { menuId: menuId });

	return data;
};

export const ContentAPI = {
	getMenu
};
