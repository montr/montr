import { DataMenu, UserContextProps, withUserContext } from "@montr-core/components";
import { IMenu } from "@montr-core/models";
import { Patterns } from "@montr-idx/module";
import React from "react";

interface Props extends UserContextProps {
	menuId: string;
	mode?: 'horizontal' | 'vertical' | 'inline';
}

export class WrappedMainMenu extends React.Component<Props> {

	getUserMenu = (): IMenu[] => {

		const { user, login, logout } = this.props;

		console.log(user);

		const result: IMenu[] = [];

		if (user) {
			const userName = user.profile.name ?? user.profile.email;

			result.push({
				name: userName,
				icon: "user",
				items: [
					{ name: "Profile", route: Patterns.profile },
					{ name: "Logout", /* icon: "logout", */ onClick: logout },
				]
			});
		} else {
			result.push({
				name: "Login",
				icon: "login",
				onClick: login
			});
		}

		return result;
	};

	render = () => {
		const { menuId, mode } = this.props;

		const theme = "light";

		return (
			<DataMenu
				menuId={menuId}
				theme={theme}
				mode={mode}
				tail={this.getUserMenu()}
			/>
		);
	};
}

export const MainMenu = withUserContext(WrappedMainMenu);
