import { Tabs } from "antd";
import React from "react";
import { Icon } from ".";
import { DataPane, DataPaneProps } from "../models";
import { ComponentFactory } from "./component-factory";

interface Props<TModel> {
	tabKey?: string;
	panes?: DataPane<TModel>[],
	onTabChange?: (tabKey: string) => void,
	disabled?: (pane: DataPane<TModel>, index: number) => boolean,
	paneProps?: DataPaneProps<TModel>;
}

export class DataTabs<TModel> extends React.Component<Props<TModel>> {

	render = (): React.ReactNode => {
		const { tabKey, panes, onTabChange, disabled, paneProps } = this.props;

		if (!panes) return null;

		return <Tabs size="small" activeKey={tabKey} onChange={onTabChange}>
			{panes.map((pane, index) => {
				return (
					<Tabs.TabPane key={pane.key}
						tab={<>{pane.icon && Icon.get(pane.icon)}{pane.name}</>}
						disabled={disabled ? disabled(pane, index) : false}>
						{ComponentFactory.createComponent(pane.component, { ...paneProps, ...pane.props })}
					</Tabs.TabPane>
				);
			})}
		</Tabs>;
	};
}
