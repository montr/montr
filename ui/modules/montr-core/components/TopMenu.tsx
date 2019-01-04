import * as React from "react";

import { Menu, Icon } from "antd";

import { IMenu, ContentAPI } from "../api";
import { AuthService } from "../services/AuthService";

interface Props {
}

interface State {
	menu: IMenu;
	user?: any;
}

export class TopMenu extends React.Component<Props, State> {

	public authService: AuthService;
	private shouldCancel: boolean;

	constructor(props: Props) {
		super(props);

		this.state = {
			menu: { items: [] },
		};

		this.shouldCancel = false;

		this.authService = new AuthService();
		this.authService.addUserLoaded((user) => {
			console.log("USER LOADED", window.frameElement, user);
			//if (!this.shouldCancel) {
			this.setState({ user });
			//}
		});
	}

	public componentDidMount() {
		// todo: move to special route and component for login
		if (window.location.href.indexOf("/signin-oidc") != -1) {
			this.authService.signinRedirectCallback().then((user) => {
				window.location.href = "/";
			}).catch(function (e) {
				console.error(e);
			});
		}

		ContentAPI
			.getMenu("TopMenu")
			.then((data: IMenu) => {
				this.setState({ menu: data });
			});

		this.getUser();
	}

	public login = () => {
		this.authService.login();
	};

	public componentWillUnmount() {
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
		this.authService.getUser().then((user: any) => {

			if (user) {
				if (!this.shouldCancel) {
					this.setState({ user });
				}
			}
			/* else {
				console.log("LOGIN SILENT", window.frameElement, user);
				this.authService.loginSilent().then((user) => {
					console.log("LOGIN SILENT RESULT", window.frameElement, user);
				});
			} */
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
				<Menu.Item key="user:rt">
					<span><Icon type="sync" /><a onClick={this.renewToken}>Обновить токен</a></span>
				</Menu.Item>
				{userMenu}
			</Menu>
		);
	}
}
