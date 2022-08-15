import { DataBreadcrumb } from "@montr-core/components";
import { IMenu } from "@montr-core/models";
import * as React from "react";
import { Classifier, ClassifierType } from "../models";
import { Patterns, RouteBuilder } from "../module";

interface Props {
	type?: ClassifierType;
	types?: ClassifierType[];
	item?: Classifier;
}

export class ClassifierBreadcrumb extends React.Component<Props> {
	render = (): React.ReactNode => {
		const { type, types, item } = this.props;

		// todo: localize
		const items: IMenu[] = [
			{ name: "Classifiers", route: Patterns.searchClassifierType }
		];

		if (type && type.code) {
			const typeItem: IMenu = { name: type.name, route: RouteBuilder.searchClassifier(type.code) };

			if (types) {
				typeItem.items = types.map(x => {
					return { name: x.name, route: RouteBuilder.searchClassifier(x.code) };
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
	};
}
