import { DataMenu, UserContextProps, withUserContext } from "@montr-core/components";
import { IMenu } from "@montr-core/models";
import { Patterns } from "@montr-idx/module";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components";
import React from "react";

interface Props extends UserContextProps, CompanyContextProps {
	menuId: string;
	mode?: 'horizontal' | 'vertical' | 'inline';
}

export class WrappedMainMenu extends React.Component<Props> {

	getUserMenu = (): IMenu[] => {

		const { user, login, logout,
			currentCompany, companyList, registerCompany, manageCompany, switchCompany } = this.props;

		const result: IMenu[] = [];

		if (user) {

			if (currentCompany) {

				const companyMenu: IMenu = {
					name: currentCompany.name,
					icon: "setting",
					items: [
						{ name: "Manage company", onClick: manageCompany }
					]
				};

				if (companyList) {
					companyMenu.items.push({
						name: "Switch company",
						items: companyList.map(item => {
							return {
								name: item.name,
								onClick: () => { switchCompany(item.uid); }
							};
						})
					});
				}

				companyMenu.items.push(
					{ name: "Register company", onClick: registerCompany }
				);

				result.push(companyMenu);
			}

			result.push({
				name: user.profile.name ?? user.profile.email,
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

export const MainMenu = withCompanyContext(withUserContext(WrappedMainMenu));