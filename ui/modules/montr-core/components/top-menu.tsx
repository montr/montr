import * as React from "react";
import { Menu } from "antd";
import { IMenu, ContentAPI } from "../api";
import { UserMenu } from "./";
import { MenuProps } from "antd/lib/menu";

interface Props {
	menuId: string;
	head?: React.ReactElement<MenuProps>;
}

interface State {
	menu: IMenu;
}

export class TopMenu extends React.Component<Props, State> {

	constructor(props: any) {
		super(props);

		this.state = {
			menu: { items: [] },
		};
	}

	public componentDidMount() {
		const { menuId } = this.props;

		ContentAPI.getMenu(menuId).then((data: IMenu) => {
			this.setState({ menu: data });
		});
	}

	render() {

		const { menu } = this.state;

		return (
			<Menu theme="light" mode="horizontal" style={{ lineHeight: "64px" }}>

				{menu.items && menu.items.map((item) => {
					return (
						<Menu.Item key={item.id}>
							<a href={item.url}>{item.name}</a>
						</Menu.Item>
					);
				})}

				<UserMenu style={{ float: "right" }} head={this.props.head} />

			</Menu>
		);
	}
}
