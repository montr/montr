import { Patterns } from "@montr-core/module";
import { Menu } from "antd";
import { MenuProps } from "antd/es/menu";
import * as React from "react";
import { Link } from "react-router-dom";
import { Icon, UserContextProps, withUserContext } from "./";

interface Props {
	strongTitle?: string;
	head?: React.ReactElement<MenuProps>;
	// tail?: React.ReactElement<MenuProps>;
}

// todo: remove, all code in MainMenu
class _UserMenu extends React.Component<MenuProps & UserContextProps & Props> {

	render = () => {

		const { strongTitle, user, login, logout, head, /* tail, */ ...props } = this.props;

		if (user) {

			const userTitle = user.profile.name ?? user.profile.email;

			const title = (strongTitle)
				? (<span>
					{/* <Icon type="user" /> */}
					<div style={{ lineHeight: '14px' }}>&#xA0;</div>
					<div style={{ lineHeight: '18px' }}><strong>{strongTitle}</strong></div>
					<div style={{ lineHeight: '32px' }}>{userTitle}</div>
				</span>) : userTitle;

			return (
				<Menu.SubMenu key="user:menu" {...props} className="user-menu" title={title}>

					{head}

					<Menu.Item key="user:1">
						<Link to={Patterns.profile}>{userTitle}</Link>
					</Menu.Item>
					<Menu.Item key="user:logout">
						<a onClick={logout}>Выйти</a>
					</Menu.Item>

					{/* {tail} */}

				</Menu.SubMenu>
			);
		}

		return (
			<Menu.SubMenu key="user:login" {...props} title={
				<span>{Icon.get("login")}<a onClick={login}>Войти</a></span>
			} />
		);
	};
}

export const UserMenu = withUserContext(_UserMenu);
