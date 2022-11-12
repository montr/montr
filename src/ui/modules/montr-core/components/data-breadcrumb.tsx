import { Breadcrumb, Dropdown } from "antd";
import * as React from "react";
import { Link, Navigate } from "react-router-dom";
import { IMenu } from "../models";
import { buildMenuIds, handleMenuClick, NavigateLocation } from "./data-menu";
import { Icon } from "./icon";

interface Props {
	items: IMenu[];
}

interface State {
	navigateTo?: NavigateLocation;
}

export class DataBreadcrumb extends React.Component<Props, State> {

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	// todo: use on click
	getItemRoute = (item: IMenu): string => {
		if (typeof item.route == "string") {
			return item.route as string;
		}

		return item.route();
	};

	getItem = (value: IMenu, index: number): React.ReactNode => {

		if (value.items) {

			return (
				<Breadcrumb.Item key={index}>
					<Dropdown trigger={['click']}
						menu={{
							items: value.items.map(item => ({
								key: item.id, label: item.name
							})),
							onClick: (item) => {
								this.onClick(value.items.find(x => x.id == item.key));
							}
						}}>
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

	onClick = (item: IMenu): void => {
		const navigateTo = handleMenuClick(item);

		if (navigateTo) {
			this.setState({ navigateTo });
		}
	};

	render = (): React.ReactNode => {
		const { items } = this.props,
			{ navigateTo } = this.state;

		if (navigateTo && !navigateTo.navigated) {
			navigateTo.navigated = true;
			return <Navigate to={navigateTo.path} />;
		}

		buildMenuIds(items);

		return (
			<Breadcrumb>
				<Breadcrumb.Item>{Icon.Home}</Breadcrumb.Item>
				{items.map(this.getItem)}
			</Breadcrumb>
		);
	};
}
