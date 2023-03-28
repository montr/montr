import { Breadcrumb } from "antd";
import { BreadcrumbItemType, ItemType } from "antd/es/breadcrumb/Breadcrumb";
import * as React from "react";
import { Navigate } from "react-router-dom";
import { IMenu } from "../models";
import { NavigateLocation, buildMenuIds, canHandleMenuClick, handleMenuClick } from "./data-menu";

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

	getItem = (value: IMenu): ItemType => {

		const result: BreadcrumbItemType = {
			title: canHandleMenuClick(value) ? <a>{value.name}</a> : value.name,
			/* onClick: () => {
				this.onClick(value);
			} */
		};

		if (value.items) {
			result.menu = {
				items: value.items.map(item => ({
					key: item.id, label: item.name
				})),
				onClick: (info) => {
					this.onClick(value.items.find(x => x.id == info.key));
				}
			};
		} else {
			result.onClick = () => {
				this.onClick(value);
			};
		}

		return result;
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
			<Breadcrumb items={items.map(this.getItem)} />
		);
	};
}
