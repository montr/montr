import { DataFormOptions, extendNamePath } from "@montr-core/components";
import { DataFieldFactory } from "@montr-core/components/data-field-factory";
import { IDataField } from "@montr-core/models";
import { Space } from "antd";
import React from "react";
import { AutomationAction } from "../models";
import { AutomationService } from "../services";
import { AutomationContextProps, withAutomationContext } from "./automation-context";
import { AutomationItemProps } from "./automation-field-factory";

interface Props extends AutomationItemProps, AutomationContextProps {
	action: AutomationAction;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

/**
 * Generic automation item component.
 * Used if specific component for action is not registered with AutomationActionFactory.register method.
 * Simple load action type metadata and displays form fields.
 */
class WrappedAutomationActionItem extends React.Component<Props, State> {

	private readonly automationService = new AutomationService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchMetadata();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.automationService.abort();
	};

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.action?.type != prevProps.action?.type) {
			await this.fetchMetadata();
		}
	};

	fetchMetadata = async (): Promise<void> => {
		const { data, action } = this.props;

		if (action?.type) {
			const fields = await this.automationService.actionMetadata(data.entityTypeCode, action);

			this.setState({ loading: false, fields });
		} else {
			this.setState({ loading: false });
		}
	};

	render = () => {
		const { typeSelector, item, options } = this.props,
			{ fields } = this.state;

		const innerOptions: DataFormOptions = {
			namePathPrefix: extendNamePath(item.name, ["props"]),
			...options
		};

		return (<>
			<Space>
				{typeSelector}
			</Space>

			{fields && fields.map(field => {
				const factory = DataFieldFactory.get(field.type);

				return factory?.createFormItem(field, null, innerOptions);
			})}
		</>);
	};
}

export const AutomationActionItem = withAutomationContext(WrappedAutomationActionItem);
