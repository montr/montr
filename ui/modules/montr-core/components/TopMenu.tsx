import * as React from "react";

import { Menu, Icon } from "antd";

import { IAccountInfo, AccountAPI, IMenu, ContentAPI } from "../api";
import { AuthService } from "../services/AuthService";

interface Props {
}

interface State {
	menu: IMenu;
	data: IAccountInfo;
	user?: any;
}

export class TopMenu extends React.Component<Props, State> {

	public authService: AuthService;
	private shouldCancel: boolean;

	constructor(props: Props) {
		super(props);

		this.state = {
			menu: { items: [] },
			data: { claims: {} }
		};

		this.authService = new AuthService();
		this.shouldCancel = false;
	}

	public componentDidMount() {
		// todo: move to special route and component
		if (window.location.href.indexOf("/signin-oidc") != -1) {
			this.authService.signinRedirectCallback().then((user) => {
				window.location.href = "/";
			}).catch(function (e) {
				console.error(e);
			});
		}

		this.authService.getUser().then((user: any) => {
			if (user && user.access_token) {
				ContentAPI
					.getMenu("TopMenu", user.access_token)
					.then((data: IMenu) => {
						this.setState({ menu: data });
					});
			}
		});

		/* AccountAPI
			.info()
			.then((data) => {
				this.setState({ data });
			}); */

		// this.getUser();
	}

	public login = () => {
		this.authService.login();
	};

	public componentWillUnmount() {
		console.log("componentWillUnmount");
		this.shouldCancel = true;
	}

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

	public logout = () => {
		this.authService.logout();
	};

	public getUser = () => {
		this.authService.getUser().then(user => {
			if (user) {
				console.log("User has been successfully loaded from store.");
			} else {
				console.log("You are not logged in.");
			}

			if (!this.shouldCancel) {
				this.setState({ user });
			}
		});
	};

	render() {

		let { user } = this.state;

		let userMenu: React.ReactFragment;

		if (user) {
			userMenu = (
				<Menu.SubMenu style={{ float: "right" }} title={
					<span><Icon type="user" />{user.profile.name}</span>
				}>
					<Menu.Item key="user:1"><a href="http://idx.montr.io:5050/">Личный кабинет</a></Menu.Item>
					<Menu.Item key="user:rt">
						<span><Icon type="sync" /><a onClick={this.renewToken}>Обновить токен</a></span>
					</Menu.Item>
					<Menu.Item key="user:logout">
						<span><Icon type="logout" /><a onClick={this.logout}>Выйти</a></span>
					</Menu.Item>
				</Menu.SubMenu>
			);
		}
		else {
			userMenu = (
				<Menu.SubMenu style={{ float: "right" }} key="user:login" title={
					<span><Icon type="login" /><a onClick={this.login}>Войти</a></span>
				}>
				</Menu.SubMenu>
			);
		}

		return (
			<Menu theme="light" mode="horizontal" style={{ lineHeight: "64px" }}>

				{this.state.menu.items && this.state.menu.items.map((item) => {
					return (
						<Menu.Item key={item.id}>
							<a href={item.url}>{item.name}</a>
						</Menu.Item>
					);
				})}

				{userMenu}
			</Menu>
		);
	}
}
