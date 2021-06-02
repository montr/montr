import * as React from "react";
import { Link } from "react-router-dom";
import { Menu } from "antd";
import { IMenu } from "../models";
import { MenuProps } from "antd/lib/menu";
import { ContentService } from "../services/content-service";
import { NavigationService } from "@montr-core/services";
import { Icon } from "./";

interface Props extends MenuProps {
	menuId: string;
	head?: React.ReactElement<MenuProps>;
	tail?: React.ReactElement<MenuProps>;
}

interface State {
	menu?: IMenu;
	openKeys?: string[];
	selectedKeys?: string[];
}

export class DataMenu extends React.Component<Props, State> {

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
		if (parent.items) {

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

	getItemRoute = (item: IMenu): string => {
		if (typeof item.route == "string") {
			return item.route as string;
		}

		return item.route();
	};

	buildItems = (menu: IMenu, keyPrefix = ""): React.ReactElement[] => {
		return menu?.items?.map((item, index) => {

			const key = item.id ?? keyPrefix + "_" + index;

			if (item.items?.length > 0) {
				return (
					<Menu.SubMenu key={key} title={
						<span>
							{item.icon && Icon.get(item.icon)}
							<span>{item.name}</span>
						</span>
					}>
						{this.buildItems(item, key + "_")}
					</Menu.SubMenu>
				);
			}

			if (item.route) {
				return (
					<Menu.Item key={key}>
						<Link to={this.getItemRoute(item)}>
							{item.icon && Icon.get(item.icon)}
							<span className="nav-text">{item.name}</span>
						</Link>
					</Menu.Item>
				);
			}

			return (
				<Menu.Item key={key}>
					<a href={item.url}>
						{item.icon && Icon.get(item.icon)}
						<span className="nav-text">{item.name}</span>
					</a>
				</Menu.Item>
			);
		});
	};

	render = (): React.ReactNode => {

		const { menuId: _, head, tail, ...props } = this.props,
			{ menu, openKeys, selectedKeys } = this.state;

		return (<>
			{menu && <Menu
				defaultOpenKeys={openKeys}
				defaultSelectedKeys={selectedKeys}
				{...props}>

				{head}

				{this.buildItems(menu)}

				{tail}

			</Menu>}
		</>);
	};
}
