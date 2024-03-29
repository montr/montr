import React from "react";
import { AutomationAction } from "../models/automation";
import { AutomationActionItem } from "./automation-action-item";
import { AutomationActionFactory, AutomationItemProps } from "./automation-field-factory";

interface Props extends AutomationItemProps {
	value?: AutomationAction;
}

export class AutomationActionItemWrapper extends React.Component<Props> {

	render = () => {
		const { value } = this.props;

		if (value?.type) {
			const factory = AutomationActionFactory.get(value?.type);

			if (factory) {
				return factory.createFormItem(value, { ...this.props });
			}
		}

		return <AutomationActionItem action={value} {...this.props} />;
	};
}
