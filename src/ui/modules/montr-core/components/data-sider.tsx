import { DataPane, DataPaneProps } from "@montr-core/models";
import { Collapse } from "antd";
import React from "react";
import { Icon } from ".";
import { ComponentFactory } from "./component-factory";

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
				ghost
				expandIconPosition={"end"}
				defaultActiveKey={panes[0].key}
				items={panes.map((pane, _) => {
					return (
						{
							key: pane.key,
							label: <>{pane.icon && Icon.get(pane.icon)}{pane.name}</>,
							children: ComponentFactory.createComponent(pane.component, { ...paneProps, ...pane.props })
						}
					);
				})}>

			</Collapse>
		);
	};
}
