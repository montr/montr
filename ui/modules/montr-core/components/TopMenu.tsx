import * as React from "react";

import { Menu, Icon } from "antd";

import { IAccountInfo, AccountAPI, IMenu, ContentAPI } from "../api";

interface Props {
}

interface State {
	menu: IMenu;
	data: IAccountInfo;
}

export class TopMenu extends React.Component<Props, State> {

	constructor(props: Props) {
		super(props);

		this.state = {
			menu: { items: [] },
			data: { claims: {} }
		};
	}

	componentWillMount() {
		ContentAPI
			.getMenu("TopMenu")
			.then((data: IMenu) => {
				this.setState({ menu: data });
			});

		AccountAPI
			.info()
			.then((data) => {
				this.setState({ data });
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

				<Menu.SubMenu style={{ float: "right" }} title={
					<span><Icon type="user" />{this.state.data.claims && this.state.data.claims["name"]}</span>}>
					<Menu.Item key="user:1"><a href="http://idx.montr.io:5050/">Личный кабинет</a></Menu.Item>
				</Menu.SubMenu>
			</Menu>
		);
	}
}
