import * as React from "react";

import { Menu, Icon } from "antd";

import { IMenu, ContentAPI } from "../api";
import { UserMenu } from "./";

interface State {
	menu: IMenu;
}

export class TopMenu extends React.Component<any, State> {

	constructor(props: any) {
		super(props);

		this.state = {
			menu: { items: [] },
		};
	}

	public componentDidMount() {
		ContentAPI.getMenu("TopMenu").then((data: IMenu) => {
			this.setState({ menu: data });
		});
	}

	render() {

		return (
			<Menu theme="light" mode="horizontal" style={{ lineHeight: "64px" }}>

				{this.state.menu.items && this.state.menu.items.map((item) => {
					return (
						<Menu.Item key={item.id}>
							<a href={item.url}>{item.name}</a>
						</Menu.Item>
					);
				})}

				<UserMenu style={{ float: "right" }} />

			</Menu>
		);
	}
}
