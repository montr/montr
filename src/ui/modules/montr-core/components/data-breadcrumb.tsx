import * as React from "react";
import { Breadcrumb, Dropdown, Menu } from "antd";
import { IMenu } from "../models";
import { Link } from "react-router-dom";
import { Icon } from ".";

interface Props {
	items: IMenu[];
}

export class DataBreadcrumb extends React.Component<Props> {

	getItemRoute = (item: IMenu): string => {
		if (typeof item.route == "string") {
			return item.route as string;
		}

		return item.route();
	};

	getItem = (value: IMenu, index: number) => {

		if (value.items) {

			const overlay = (
				<Menu>
					{value.items.map((x, idx) => <Menu.Item key={`$${index}.${idx}`}>
						<Link to={this.getItemRoute(x)}>{x.name}</Link>
					</Menu.Item>)}
				</Menu>
			);

			return (
				<Breadcrumb.Item key={index}>
					<Dropdown overlay={overlay} trigger={['click']}>
						<a className="ant-dropdown-link">
							{value.name} {Icon.Down}
						</a>
					</Dropdown>
				</Breadcrumb.Item>
			);
		}

		return (
			<Breadcrumb.Item key={index}>
				{
					value.route
						? <Link to={this.getItemRoute(value)}>{value.name}</Link>
						: (value.name)
				}
			</Breadcrumb.Item>
		);
	};

	render = () => {
		return (
			<Breadcrumb>
				<Breadcrumb.Item>{Icon.Home}</Breadcrumb.Item>
				{this.props.items.map(this.getItem)}
			</Breadcrumb>
		);
	};
};
