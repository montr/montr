import { UserContextProps, withUserContext } from "@montr-core/components";
import { DataMenu } from "@montr-core/components/data-menu";
import { IMenu } from "@montr-core/models/menu";
import { ProfileModel } from "@montr-idx/models/profile-model";
import { Patterns } from "@montr-idx/module";
import { ProfileService } from "@montr-idx/services/profile-service";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components/company-context";
import React from "react";

interface Props extends UserContextProps, CompanyContextProps {
	menuId: string;
	mode?: 'horizontal' | 'vertical' | 'inline';
}

interface State {
	loading: boolean;
	profile?: ProfileModel;
}

export class WrappedMainMenu extends React.Component<Props, State> {

	private readonly profileService = new ProfileService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.profileService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { user } = this.props;

		const profile = user ? await this.profileService.get() : null;

		this.setState({ loading: false, profile });
	};

	getUserMenu = (): IMenu[] => {

		const { user, login, logout,
			currentCompany, companyList, registerCompany, manageCompany, switchCompany } = this.props,
			{ profile } = this.state;

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
				name: <span>{profile?.displayName ?? profile?.userName}</span>,
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
