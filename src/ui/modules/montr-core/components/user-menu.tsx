import * as React from "react";
import { Menu } from "antd";
import { UserContextProps, withUserContext } from "./";
import { MenuProps } from "antd/lib/menu";
import { Icons } from "./icons";

interface Props {
	strongTitle?: string;
	head?: React.ReactElement<MenuProps>;
	tail?: React.ReactElement<MenuProps>;
}

class _UserMenu extends React.Component<MenuProps & UserContextProps & Props> {

	render = () => {

		let { strongTitle, user, login, logout, head, tail, ...props } = this.props;

		if (user) {

			let title = (strongTitle)
				? (<span>
					{/* <Icon type="user" /> */}
					<div style={{ lineHeight: '14px' }}>&#xA0;</div>
					<div style={{ lineHeight: '18px' }}><strong>{strongTitle}</strong></div>
					<div style={{ lineHeight: '32px' }}>{user.profile.name}</div>
				</span>) : user.profile.name;

			return (
				<Menu.SubMenu {...props} className="user-menu" title={title}>

					{head}

					<Menu.Item key="user:0" className="menu-header" disabled>
						{/* <Icon type="user" /> */}
						<strong>{user.profile.name}</strong>
					</Menu.Item>
					<Menu.Item key="user:1">
						{/* todo: use routes */}
						<a href="/profile">Настройки пользователя</a>
					</Menu.Item>
					<Menu.Item key="user:logout">
						<a onClick={logout}>Выйти</a>
					</Menu.Item>

					{tail}

				</Menu.SubMenu>
			);
		}

		return (
			<Menu.SubMenu {...props} title={
				<span>{Icons.get("login")}<a onClick={login}>Войти</a></span>
			} />
		);
	};
}

export const UserMenu = withUserContext(_UserMenu);
