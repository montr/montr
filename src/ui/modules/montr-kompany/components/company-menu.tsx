import Menu from "antd/es/menu";
import { MenuItemProps } from "antd/es/menu/MenuItem";
import * as React from "react";
import { Company } from "../models/company";
import { CompanyContextProps, withCompanyContext } from "./company-context";

class WrappedCompanyMenu extends React.Component<MenuItemProps & CompanyContextProps> {

	render() {

		const { currentCompany, companyList, registerCompany, manageCompany, switchCompany, ...menuProps } = this.props;
		const { title, onClick, onMouseLeave, onMouseEnter, ...dividerProps } = menuProps;

		if (currentCompany) {
			return <>
				<Menu.Item key="company:header" className="menu-header" disabled {...menuProps}>
					{/* <Icon type="team" /> */}
					<strong>{currentCompany.name}</strong>
				</Menu.Item>
				<Menu.Item key="company:settings" {...menuProps}>
					<a onClick={manageCompany}>Настройки организации</a>
				</Menu.Item>
				<Menu.SubMenu key="company:switch" {...menuProps} title="Переключить организацию &#xA0; &#xA0;">

					{Array.isArray(companyList) && companyList.map((item: Company) => {
						return (
							<Menu.Item key={`company:${item.uid.toString()}`}>
								<a onClick={(e) => {
									e.preventDefault();
									switchCompany(item.uid);
								}}>{item.name}</a>
							</Menu.Item>
						);
					})}
					<Menu.Divider />
					<Menu.Item key="company:add">
						<a onClick={registerCompany}>Зарегистрировать организацию</a>
					</Menu.Item>
				</Menu.SubMenu>

				<Menu.Divider {...dividerProps} />
			</>;
		}

		return null;
	}
}

export const CompanyMenu = withCompanyContext(WrappedCompanyMenu);
