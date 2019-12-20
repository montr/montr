import * as React from "react";
import { Breadcrumb, Dropdown, Menu } from "antd";
import { HomeOutlined, DownOutlined } from "@ant-design/icons";
import { IMenu } from "@montr-core/models";
import { Link } from "react-router-dom";

interface IProps {
	items: IMenu[];
}

export class DataBreadcrumb extends React.Component<IProps> {

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
							{value.name} <DownOutlined />
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

	public render() {
		return (
			<Breadcrumb>
				<Breadcrumb.Item><HomeOutlined /></Breadcrumb.Item>
				{this.props.items.map(this.getItem)}
			</Breadcrumb>
		);
	}
};
