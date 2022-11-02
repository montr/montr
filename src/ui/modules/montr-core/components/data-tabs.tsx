import { Tabs, Tooltip } from "antd";
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

		return <Tabs
			size="small"
			activeKey={tabKey}
			onChange={onTabChange}
			items={panes?.map((pane, index) => {
				return ({
					key: pane.key,
					label: <span>
						{pane.icon && Icon.get(pane.icon)}
						{pane.description ? <>&#xA0;<Tooltip title={pane.description}>{pane.name}</Tooltip></> : pane.name}
					</span>,
					disabled: disabled ? disabled(pane, index) : false,
					children: ComponentFactory.createComponent(pane.component, { ...paneProps, ...pane.props })
				});
			})} />;
	};
}
