import { UserMenu } from "@montr-core/components";
import { MenuProps } from "antd/lib/menu";
import * as React from "react";
import { CompanyContextProps, withCompanyContext } from ".";
import { CompanyMenu } from "./company-menu";

// todo: remove, all code in MainMenu
class _UserWithCompanyMenu extends React.Component<MenuProps & CompanyContextProps> {
	render = () => {

		const { currentCompany, companyList, registerCompany, manageCompany, switchCompany, onMouseEnter, ...props } = this.props,
			companyTitle = currentCompany ? currentCompany.name : null;

		return (
			<UserMenu
				{...props}
				strongTitle={companyTitle}
				head={
					<CompanyMenu />
				} />
		);
	};
}

export const UserWithCompanyMenu = withCompanyContext(_UserWithCompanyMenu);
