import * as React from "react";

import { Menu, Icon } from "antd";

import { AuthService } from "../services/AuthService";
import { MenuProps } from "antd/lib/menu";

interface State {
	// todo: create User class
	user?: any;
}

export class UserMenu extends React.Component<MenuProps, State> {

	public authService: AuthService;

	constructor(props: MenuProps) {
		super(props);

		this.state = {
		};

		this.authService = new AuthService();
	}

	public componentDidMount() {
		this.getUser();
	}

	public login = () => {
		this.authService.login();
	};

	public logout = () => {
		this.authService.logout();
	};

	public renewToken = () => {
		this.authService
			.renewToken()
			.then(user => {
				console.log("Token has been sucessfully renewed. :-)");
				this.getUser();
			})
			.catch(error => {
				console.log(error);
			});
	};

	public getUser = () => {
		this.authService.getUser().then((user: any) => {
			this.setState({ user });
		});
	};

	render() {

		let { user } = this.state;

		if (user) {
			return (
				<Menu.SubMenu {... this.props} title={
					<span><Icon type="user" />{user.profile.name}</span>
				}>
					<Menu.Item key="user:1">
						<a href="http://idx.montr.io:5050/">Личный кабинет</a>
					</Menu.Item>

					<Menu.Item key="user:rt">
						<a onClick={this.renewToken}>Обновить токен</a>
					</Menu.Item>

					<Menu.Item key="user:logout">
						<a onClick={this.logout}>Выйти</a>
					</Menu.Item>
				</Menu.SubMenu>
			);
		}

		return (
			<Menu.SubMenu {... this.props} title={
				<span><Icon type="login" /><a onClick={this.login}>Войти</a></span>
			} />
		);
	}
}
