import * as React from "react";
import { Menu } from "antd";
import { IMenu, ContentAPI } from "../api";
import { MenuProps } from "antd/lib/menu";

interface Props {
	menuId: string;
	head?: React.ReactElement<MenuProps>;
	tail?: React.ReactElement<MenuProps>;
}

interface State {
	menu: IMenu;
}

export class TopMenu extends React.Component<MenuProps & Props, State> {

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

		const { menuId, head, tail, ...props } = this.props;
		const { menu } = this.state;

		return (
			<Menu {...props}>

				{head}

				{menu.items && menu.items.map((item) => {
					return (
						<Menu.Item key={item.id}>
							<a href={item.url}>{item.name}</a>
						</Menu.Item>
					);
				})}

				{tail}

			</Menu>
		);
	}
}
