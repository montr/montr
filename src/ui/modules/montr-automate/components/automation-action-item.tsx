import { DataFieldFactory, DataFormOptions } from "@montr-core/components";
import { IDataField } from "@montr-core/models";
import { Space } from "antd";
import React from "react";
import { AutomationContextProps, AutomationItemProps } from ".";
import { AutomationAction } from "../models";
import { AutomationService } from "../services";
import { withAutomationContext } from "./automation-context";

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
		if (this.props.action !== prevProps.action) {
			// await this.fetchMetadata();
		}
	};

	fetchMetadata = async (): Promise<void> => {
		const { data, action } = this.props;

		if (action?.type) {
			const fields = await this.automationService.metadata(data.entityTypeCode, action.type, null);

			this.setState({ loading: false, fields });
		} else {
			this.setState({ loading: false });
		}
	};

	render = () => {
		const { typeSelector, item, options } = this.props,
			{ fields } = this.state;

		const innerOptions: DataFormOptions = { namePathPrefix: [item.name, "props"], ...options };

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
