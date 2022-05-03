import { NavigationService } from "@montr-core/services";
import { Menu } from "antd";
import { MenuProps } from "antd/lib/menu";
import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { IMenu } from "../models";
import { ContentService } from "../services/content-service";
import { Icon } from "./";

interface Props extends MenuProps, RouteComponentProps {
	menuId: string;
	head?: React.ReactElement<MenuProps>;
	tail?: React.ReactElement<MenuProps>;
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

	buildItems = (menu: IMenu, keyPrefix = ""): MenuItem[] => {
		return menu?.items?.map((item, index) => {

			const key = item.id ?? (keyPrefix + "_" + index);

			return {
				key: key,
				label: item.name,
				icon: item.icon && Icon.get(item.icon),
				children: (item.items?.length > 0) ? this.buildItems(item, key + "_") : null,
				onClick: () => this.onClick(item)
			} as MenuItem;
		});
	};

	onClick = (item: IMenu): void => {
		if (item.route) {
			const route =
				(typeof item.route == "string")
					? item.route as string
					: item.route();

			this.props.history.push(route);
		}
		else if (item.url) {
			window.location.href = item.url;
		}
	};

	render = (): React.ReactNode => {

		const { menuId, head, tail, staticContext: _, ...props } = this.props,
			{ menu, openKeys, selectedKeys } = this.state;

		const items = this.buildItems(menu, menuId);

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

export const DataMenu = withRouter(WrappedDataMenu);
