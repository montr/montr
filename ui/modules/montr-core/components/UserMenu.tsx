import * as React from "react";

import { User } from "oidc-client";

import { Menu, Icon, message } from "antd";

import { AuthService } from "../services/AuthService";
import { MenuProps } from "antd/lib/menu";

interface State {
	// todo: create User class
	user?: User;
}

export class UserMenu extends React.Component<MenuProps, State> {

	_authService: AuthService;

	constructor(props: MenuProps) {
		super(props);

		this.state = {
		};

		this._authService = new AuthService();
	}

	componentDidMount() {
		this.getUser(true);

		this._authService.onAuthenticated((user: User) => {
			this.setState({ user });
			// this.getUser(false);
		})
	}

	login = () => {
		this._authService.login()
			.catch(error => { // todo: use logger here and below
				console.log("login error", error);
			});
	};

	loginSilent = () => {
		this._authService.loginSilent()
			.catch(error => {
				console.log("loginSilent error", error);
			});
	};

	logout = () => {
		this._authService.logout()
			.catch(error => {
				console.log("logout error", error);
			});
	};

	getUser = (withLoginSilent: boolean) => {
		this._authService.getUser()
			.then((user: User) => {
				this.setState({ user });

				if (withLoginSilent) {
					if (!user || user.expired) {
						this.loginSilent();
					}
				}
			}).catch(error => {
				console.log("getUser error", error);
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
						<a href="http://idx.montr.io:5050/Identity/Account/Manage">Личный кабинет</a>
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
