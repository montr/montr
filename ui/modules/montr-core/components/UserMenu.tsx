import * as React from "react";
import { Menu, Icon } from "antd";
import { UserContextProps, withUserContext } from "./"
import { MenuProps } from "antd/lib/menu";

class _UserMenu extends React.Component<MenuProps & UserContextProps> {
	render() {

		let { user, login, logout, ...props } = this.props;

		if (user) {
			return (
				<Menu.SubMenu {...props} title={
					<span><Icon type="user" />{user.profile.name}</span>
				}>
					<Menu.Item key="user:1">
						<a href="http://idx.montr.io:5050/Identity/Account/Manage">Личный кабинет</a>
					</Menu.Item>

					<Menu.Item key="user:logout">
						<a onClick={logout}>Выйти</a>
					</Menu.Item>
				</Menu.SubMenu>
			);
		}

		return (
			<Menu.SubMenu {...props} title={
				<span><Icon type="login" /><a onClick={login}>Войти</a></span>
			} />
		);
	}
}

export const UserMenu = withUserContext(_UserMenu);
