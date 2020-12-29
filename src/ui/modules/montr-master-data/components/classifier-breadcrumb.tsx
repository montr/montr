import * as React from "react";
import { DataBreadcrumb } from "@montr-core/components";
import { IMenu } from "@montr-core/models";
import { ClassifierType, Classifier } from "../models";

interface Props {
	type?: ClassifierType;
	types?: ClassifierType[];
	item?: Classifier;
}

export class ClassifierBreadcrumb extends React.Component<Props> {
	public render() {
		const { type, types, item } = this.props;

		// todo: localize
		// todo: here and below use routes class
		const items: IMenu[] = [
			{ name: "Классификаторы", route: `/classifiers/` }
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
