import * as React from "react";
import { MenuProps } from "antd/lib/menu";
import { UserMenu } from "@montr-core/components";
import { CompanyContextProps, withCompanyContext, CompanyMenu } from ".";

class _UserWithCompanyMenu extends React.Component<MenuProps & CompanyContextProps> {
	render = () => {

		const { currentCompany, companyList, switchCompany, onMouseEnter, ...props } = this.props,
			companyTitle = currentCompany ? currentCompany.name : null;

		return (
			<UserMenu
				{...props}
				strongTitle={companyTitle}
				head={
					<CompanyMenu />
				} />
		);
	}
}

export const UserWithCompanyMenu = withCompanyContext(_UserWithCompanyMenu);
