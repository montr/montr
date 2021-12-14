import { DataPane, DataPaneProps } from "@montr-core/models";
import { ComponentRegistry } from "@montr-core/services";
import { Collapse } from "antd";
import React from "react";
import { Icon } from ".";

interface Props<TModel> {
	panes?: DataPane<TModel>[],
	paneProps?: DataPaneProps<TModel>;
}

export class DataSider<TModel> extends React.Component<Props<TModel>> {

	render = (): React.ReactNode => {

		const { panes, paneProps } = this.props;

		if (!panes) return null;

		return (
			<Collapse
				bordered={false} ghost
				expandIconPosition={"right"}
				defaultActiveKey={panes[0].key}>
				{panes.map((pane, _) => {

					let component = undefined;

					if (pane.component) {
						const componentClass = ComponentRegistry.getComponent(pane.component);

						if (componentClass) {
							component = React.createElement(componentClass, { ...paneProps, ...pane.props });
						} else {
							console.warn(`Warning: Component '${pane.component}' is not found.`);
						}
					}

					return (
						<Collapse.Panel
							key={pane.key}
							header={<>{pane.icon && Icon.get(pane.icon)}{pane.name}</>}>
							{component}
						</Collapse.Panel>
					);
				})}
			</Collapse>
		);
	};
}
