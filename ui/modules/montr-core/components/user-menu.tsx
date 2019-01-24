import * as React from "react";
import { Menu, Icon } from "antd";
import { UserContextProps, withUserContext } from "./"
import { MenuProps } from "antd/lib/menu";

interface UserMenuProps {
	head?: React.ReactElement<MenuProps>;
}

class _UserMenu extends React.Component<MenuProps & UserContextProps & UserMenuProps> {

	render() {

		let { user, login, logout, head, ...props } = this.props;

		if (user) {
			return (
				<Menu.SubMenu {...props} title={
					<span>
						{/* <Icon type="user" /> */}
						<div style={{ lineHeight: '14px' }}>&#xA0;</div>
						<div style={{ lineHeight: '18px' }}><strong>{user.profile.name}</strong></div>
						<div style={{ lineHeight: '32px' }}>{user.profile.email}</div>
					</span>
				}>

					{head}

					<Menu.Item key="user:0" disabled>
						{/* <Icon type="user" /> */}
						<strong>{user.profile.name}</strong>
					</Menu.Item>
					<Menu.Item key="user:1">
						<a href="http://idx.montr.io:5050/Identity/Account/Manage">Настройки пользователя</a>
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
