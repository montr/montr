import * as React from "react";
import Menu, { MenuProps } from "antd/lib/menu";
import { ICompany } from "../models";
import { withCompanyContext, CompanyContextProps } from ".";

class _CompanyMenu extends React.Component<MenuProps & CompanyContextProps> {

	render() {

		const { currentCompany, companyList, switchCompany, onMouseEnter, ...props } = this.props;

		if (currentCompany) {
			return <>
				<Menu.Item key="company:header" className="menu-header" disabled {...props}>
					{/* <Icon type="team" /> */}
					<strong>{currentCompany.name}</strong>
				</Menu.Item>
				<Menu.Item key="company:settings" {...props}>
					<a href="">Настройки организации</a>
				</Menu.Item>
				<Menu.SubMenu key="company:switch" {...props} title="Переключить организацию &#xA0; &#xA0;">

					{Array.isArray(companyList) && companyList.map((item: ICompany) => {
						return (
							<Menu.Item key={`company:${item.uid.toString()}`}>
								<a href="" onClick={(e) => {
									e.preventDefault();
									switchCompany(item);
								}}>{item.name}</a>
							</Menu.Item>
						);
					})}
					<Menu.Divider />
					<Menu.Item key="company:add">
						<a href="http://kompany.montr.io:5010/register/">Добавить организацию</a>
					</Menu.Item>
				</Menu.SubMenu>

				<Menu.Divider {...props} />
			</>;
		}

		return null;
	}
}

export const CompanyMenu = withCompanyContext(_CompanyMenu);
