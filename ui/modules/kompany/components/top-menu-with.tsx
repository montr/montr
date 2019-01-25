import * as React from "react";
import { withCompanyContext, CompanyMenu } from ".";
import { CompanyContextProps } from "./company-context";
import { TopMenu, UserMenu } from "@montr-core/components";

class _TopMenuWith extends React.Component<CompanyContextProps> {
	render = () => {

		let { currentCompany } = this.props;
		let companyTitle = currentCompany ? currentCompany.name : null;

		return (
			<TopMenu
				menuId="TopMenu"
				theme="light"
				mode="horizontal"
				style={{ lineHeight: "64px" }}
				tail={
					<UserMenu
						strongTitle={companyTitle}
						style={{ float: "right" }}
						head={
							<CompanyMenu />
						} />
				}
			/>
		);
	}
}

export const TopMenuWith = withCompanyContext(_TopMenuWith);
