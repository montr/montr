import * as React from "react";
import { Link } from "react-router-dom";
import { Menu, Icon } from "antd";
import { IMenu } from "../models";
import { MenuProps } from "antd/lib/menu";
import { ContentService } from "../services/content-service";

interface Props {
	menuId: string;
	head?: React.ReactElement<MenuProps>;
	tail?: React.ReactElement<MenuProps>;
}

interface State {
	menu: IMenu;
}

export class DataMenu extends React.Component<MenuProps & Props, State> {

	private _contentService = new ContentService();

	constructor(props: any) {
		super(props);

		this.state = {
			menu: { items: [] },
		};
	}

	componentDidMount = async () => {
		const { menuId } = this.props;

		this.setState({ menu: await this._contentService.getMenu(menuId) });
	}

	componentWillUnmount = async () => {
		await this._contentService.abort();
	}

	getItemRoute = (item: IMenu): string => {
		if (typeof item.route == "string") {
			return item.route as string;
		}

		return item.route();
	}

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
	}

	render() {

		const { menuId, head, tail, ...props } = this.props;
		const { menu } = this.state;

		return (
			<Menu {...props}>

				{head}

				{this.buildItems(menu)}

				{tail}

			</Menu>
		);
	}
}
