import * as React from "react";
import { DataBreadcrumb } from "@montr-core/components";
import { IMenu } from "@montr-core/models";
import { IClassifierType } from "../models";

interface IProps {
	type?: IClassifierType;
	types?: IClassifierType[];
}

export class ClassifierBreadcrumb extends React.Component<IProps> {
	public render() {
		const { type, types } = this.props;

		const items: IMenu[] = [
			{ name: "Справочники", route: `/classifiers/` }
		];

		if (type) {
			const typeItem: IMenu = { name: type.name, route: `/classifiers/${type.code}` }

			if (types) {
				typeItem.items = types.map(x => {
					return { name: x.name, route: `/classifiers/${x.code}` }
				});
			}

			items.push(typeItem);
		}

		return (
			<DataBreadcrumb items={items} />
		);
	}
}
