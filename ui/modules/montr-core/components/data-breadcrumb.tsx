import * as React from "react";
import { Breadcrumb, Icon, Dropdown, Menu } from "antd";
import { IMenu } from "@montr-core/models";
import { Link } from "react-router-dom";

interface IProps {
	items: IMenu[];
}

export class DataBreadcrumb extends React.Component<IProps> {

	private getItem = (value: IMenu, index: number) => {

		if (value.items) {

			const overlay = (
				<Menu>
					{value.items.map((x, idx) => <Menu.Item key={`$${index}.${idx}`}>
						<Link to={x.route}>{x.name}</Link>
					</Menu.Item>)}
				</Menu>
			);

			return (
				<Breadcrumb.Item key={index}>
					<Dropdown overlay={overlay} trigger={['click']}>
						<a className="ant-dropdown-link">
							{value.name} <Icon type="down" />
						</a>
					</Dropdown>
				</Breadcrumb.Item>
			);
		}

		return (
			<Breadcrumb.Item key={index}>
				<Link to={value.route}>{value.name}</Link>
			</Breadcrumb.Item>
		)
	}

	public render() {
		return (
			<Breadcrumb>
				<Breadcrumb.Item><Icon type="home" /></Breadcrumb.Item>
				{this.props.items.map(this.getItem)}
			</Breadcrumb>
		);
	}
};
