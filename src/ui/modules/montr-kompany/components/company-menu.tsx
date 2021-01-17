import * as React from "react";
import Menu from "antd/lib/menu";
import { MenuItemProps } from "antd/lib/menu/MenuItem";
import { Company } from "../models";
import { withCompanyContext, CompanyContextProps } from ".";

class WrappedCompanyMenu extends React.Component<MenuItemProps & CompanyContextProps> {

	render() {

		const { currentCompany, companyList, registerCompany, manageCompany, switchCompany, onMouseEnter, onSelect, ...props } = this.props;

		if (currentCompany) {
			return <>
				<Menu.Item key="company:header" className="menu-header" disabled {...props}>
					{/* <Icon type="team" /> */}
					<strong>{currentCompany.name}</strong>
				</Menu.Item>
				<Menu.Item key="company:settings" {...props}>
					<a onClick={manageCompany}>Настройки организации</a>
				</Menu.Item>
				<Menu.SubMenu key="company:switch" {...props} title="Переключить организацию &#xA0; &#xA0;">

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

				<Menu.Divider {...props} />
			</>;
		}

		return null;
	}
}

export const CompanyMenu = withCompanyContext(WrappedCompanyMenu);
