import * as React from "react";

import { Menu, Icon } from 'antd';
import { IAccountInfo, AccountAPI } from "../api";

interface Props {
}

interface State {
	data: IAccountInfo;
}

export class TopMenu extends React.Component<Props, State> {

	constructor(props: Props) {
		super(props);

		this.state = {
			data: { claims: {} }
		};
	}

	componentWillMount() {
		AccountAPI
			.info()
			.then((data) => {
				this.setState({ data });
			});
	}

	render() {
		return (
			<Menu theme="light" mode="horizontal" style={{ lineHeight: "64px" }}>
				<Menu.Item key="2">
					<a href="http://kompany.montr.io:5010/">kompany</a>
				</Menu.Item>
				<Menu.Item key="3">
					<a href="http://app.tendr.montr.io:5000/">app.tendr</a>
				</Menu.Item>
				<Menu.SubMenu style={{ float: 'right' }} title={<span><Icon type="user" />{this.state.data.claims["name"]}</span>}>
					<Menu.Item key="setting:1"><a href="http://idx.montr.io:5050/">Личный кабинет</a></Menu.Item>
				</Menu.SubMenu>
			</Menu>
		);
	}
}
