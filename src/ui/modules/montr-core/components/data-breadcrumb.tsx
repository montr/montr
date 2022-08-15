import { Breadcrumb, Dropdown } from "antd";
import * as React from "react";
import { Link } from "react-router-dom";
import { DataMenu, Icon } from ".";
import { IMenu } from "../models";

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

	getItem = (value: IMenu, index: number): React.ReactNode => {

		if (value.items) {

			const overlay = <DataMenu tail={value.items} />;

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

	render = (): React.ReactNode => {
		return (
			<Breadcrumb>
				<Breadcrumb.Item>{Icon.Home}</Breadcrumb.Item>
				{this.props.items.map(this.getItem)}
			</Breadcrumb>
		);
	};
}
