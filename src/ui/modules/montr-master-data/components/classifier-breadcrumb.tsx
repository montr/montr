import * as React from "react";
import { DataBreadcrumb } from "@montr-core/components";
import { IMenu } from "@montr-core/models";
import { IClassifierType, IClassifier } from "../models";

interface IProps {
	type?: IClassifierType;
	types?: IClassifierType[];
	item?: IClassifier;
}

export class ClassifierBreadcrumb extends React.Component<IProps> {
	public render() {
		const { type, types, item } = this.props;

		// todo: here and below use routes class
		const items: IMenu[] = [
			{ name: "Справочники", route: `/classifiers/` }
		];

		if (type) {
			const typeItem: IMenu = { name: type.name, route: `/classifiers/${type.code}` };

			if (types) {
				typeItem.items = types.map(x => {
					return { name: x.name, route: `/classifiers/${x.code}` };
				});
			}

			items.push(typeItem);
		}

		if (item) {
			items.push({ name: item.name });
		}

		return (
			<DataBreadcrumb items={items} />
		);
	}
}
