import * as React from "react";
import { Link } from "react-router-dom";
import { Menu, Icon } from "antd";
import { IMenu } from "../models";
import { MenuProps } from "antd/lib/menu";
import { ContentService } from "../services/content-service";
import { NavigationService } from "@montr-core/services";

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

	private _navigation = new NavigationService();
	private _contentService = new ContentService();

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentDidMount = async () => {
		const { menuId } = this.props;

		const menu = await this._contentService.getMenu(menuId),
			path = this._navigation.getPathname(),
			openKeys: string[] = [],
			selectedKeys: string[] = [];

		this.collectActiveAndOpenItems(menu, path, openKeys, selectedKeys);

		this.setState({ menu, openKeys, selectedKeys });
	};

	// returns true, if parent item contains active menu item
	collectActiveAndOpenItems = (parent: IMenu, path: string, openKeys: string[], selectedKeys: string[]): boolean => {
		if (parent.items) {
			for (let i = 0; i < parent.items.length; i++) {
				const item = parent.items[i];

				if (this.collectActiveAndOpenItems(item, path, openKeys, selectedKeys)) {
					openKeys.push(item.id);
				}

				if (typeof item.route == "string") {
					const route = item.route as string;
					if ((route == "/" && path == route) ||
						(route != "/" && path.startsWith(route))) {
						selectedKeys.push(item.id);
						return true;
					}
				}

			}
		}

		return false;
	};

	componentWillUnmount = async () => {
		await this._contentService.abort();
	};

	getItemRoute = (item: IMenu): string => {
		if (typeof item.route == "string") {
			return item.route as string;
		}

		return item.route();
	};

	buildItems = (menu: IMenu) => {
		return menu && menu.items && menu.items.map((item) => {

			if (item.items) {
				return (
					<Menu.SubMenu key={item.id} title={
						<span>
							{item.icon && <Icon type={item.icon} />}
							<span>{item.name}</span>
						</span>
					}>
						{this.buildItems(item)}
					</Menu.SubMenu>
				);
			}

			if (item.route) {
				return (
					<Menu.Item key={item.id}>
						<Link to={this.getItemRoute(item)}>
							{item.icon && <Icon type={item.icon} />}
							<span className="nav-text">{item.name}</span>
						</Link>
					</Menu.Item>
				);
			}

			return (
				<Menu.Item key={item.id}>
					<a href={item.url}>
						{item.icon && <Icon type={item.icon} />}
						<span className="nav-text">{item.name}</span>
					</a>
				</Menu.Item>
			);
		});
	};

	render() {

		const { menuId, head, tail, ...props } = this.props;
		const { menu, openKeys, selectedKeys } = this.state;

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
	}
}
