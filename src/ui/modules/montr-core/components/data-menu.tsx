import { NavigationService } from "@montr-core/services";
import { Menu } from "antd";
import { MenuProps } from "antd/lib/menu";
import * as React from "react";
import { NavigateFunction } from "react-router-dom";
import { IMenu } from "../models";
import { ContentService } from "../services/content-service";
import { Icon } from "./";
import { withNavigate } from "./react-router-wrappers";

interface Props extends MenuProps {
	menuId: string;
	// head?: React.ReactElement<MenuProps>;
	tail?: IMenu[];
	navigate: NavigateFunction;
}

interface State {
	menu?: IMenu;
	openKeys?: string[];
	selectedKeys?: string[];
}

type MenuItem = Required<MenuProps>['items'][number];

class WrappedDataMenu extends React.Component<Props, State> {

	private readonly navigation = new NavigationService();
	private readonly contentService = new ContentService();

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	fetchData = async (): Promise<void> => {
		const { menuId } = this.props;

		const menu = await this.contentService.getMenu(menuId),
			path = this.navigation.getPathname(),
			openKeys: string[] = [],
			selectedKeys: string[] = [];

		this.collectOpenAndSelectedItems(menu, path, openKeys, selectedKeys);

		this.setState({ menu, openKeys, selectedKeys });
	};

	// returns true, if parent item contains selected menu item
	collectOpenAndSelectedItems = (parent: IMenu, path: string, openKeys: string[], selectedKeys: string[]): boolean => {
		if (parent?.items) {

			let selectedItem;

			for (let i = 0; i < parent.items.length; i++) {
				const item = parent.items[i];

				if (this.collectOpenAndSelectedItems(item, path, openKeys, selectedKeys)) {
					openKeys.push(item.id);
				}

				if (typeof item.route == "string") {
					const route = item.route as string;

					const exactMatch = (path == route);
					if (exactMatch) {
						selectedItem = item;
						break;
					}

					// last start with match will win, because last matches are longest
					// otherwise should calculate match length to prevent select short matches
					const startWithMatch = (
						(route == "/" && path == route) ||
						(route != "/" && path.startsWith(route))
					);

					if (startWithMatch) {
						selectedItem = item;
					}
				}
			}

			if (selectedItem) {
				selectedKeys.push(selectedItem.id);
				return true;
			}
		}

		return false;
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.contentService.abort();
	};

	buildItems = (items: IMenu[], keyPrefix = ""): MenuItem[] => {
		return items?.map((item, index) => {

			const key = item.id ?? (keyPrefix + "_" + index);

			return {
				key: key,
				label: item.name,
				icon: item.icon && Icon.get(item.icon),
				disabled: item.disabled,
				children: (item.items?.length > 0) ? this.buildItems(item.items, key + "_") : null,
				onClick: (mi) => this.onClick(mi.domEvent, item)
			} as MenuItem;
		});
	};

	onClick = (e: React.MouseEvent<HTMLElement> | React.KeyboardEvent<HTMLElement>, item: IMenu): void => {

		e.preventDefault();

		if (item.onClick) {
			item.onClick();
		}
		else if (item.route) {
			const route =
				(typeof item.route == "string")
					? item.route as string
					: item.route();

			this.props.navigate(route);
		}
		else if (item.url) {
			window.location.href = item.url;
		}
	};

	render = (): React.ReactNode => {

		const { menuId, /* head, */ tail, navigate, ...props } = this.props,
			{ menu, openKeys, selectedKeys } = this.state;

		let menus: IMenu[] = [];

		if (menu?.items) menus = menus.concat(menu?.items);
		if (tail) menus = menus.concat(tail);

		const items = this.buildItems(menus, menuId);

		return (<>
			{menu && <Menu
				defaultOpenKeys={openKeys}
				defaultSelectedKeys={selectedKeys}
				items={items}
				{...props} />
			}
		</>);
	};
}

export const DataMenu = withNavigate(WrappedDataMenu);
