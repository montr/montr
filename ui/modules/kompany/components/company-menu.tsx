import * as React from "react";
import Menu, { MenuProps } from "antd/lib/menu";
import { ICompany, CompanyAPI } from "../api";

interface State {
	items: ICompany[];
}

export class CompanyMenu extends React.Component<MenuProps, State> {
	constructor(props: any) {
		super(props);

		this.state = {
			items: [],
		};
	}

	componentDidMount = async () => {
		const data: ICompany[] = await CompanyAPI.list();

		this.setState({ items: data });
	}

	render() {

		const { ...props } = this.props;
		const { items } = this.state;

		return <>
			<Menu.Item key="company:header" disabled {...props}>
				{/* <Icon type="team" /> */}
				<strong>ООО "Булава"</strong>
			</Menu.Item>
			<Menu.Item key="company:settings" {...props}>
				<a href="">Настройки организации</a>
			</Menu.Item>
			<Menu.SubMenu key="company:switch" {...props} title="Переключить организацию &#xA0; &#xA0;">

				{items.map((item: ICompany) => {
					return (
						<Menu.Item key={`company:${item.uid.toString()}`}>
							<a href="">{item.name}</a>
						</Menu.Item>
					);
				})}
				<Menu.Divider />
				<Menu.Item key="company:add">
					<a href="http://kompany.montr.io:5010/register/">Добавить организацию</a>
				</Menu.Item>
			</Menu.SubMenu>

			<Menu.Divider  {...props} />
		</>;
	}
}
