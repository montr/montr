import { DataFieldFactory, DataFormOptions, extendNamePath, joinNamePath } from "@montr-core/components";
import { IDataField } from "@montr-core/models";
import { Space } from "antd";
import React from "react";
import { AutomationContextProps, AutomationItemProps } from ".";
import { AutomationCondition } from "../models";
import { AutomationService } from "../services";
import { withAutomationContext } from "./automation-context";

interface Props extends AutomationItemProps, AutomationContextProps {
	id?: string; // fixit: passed by antd form?
	condition: AutomationCondition;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

/**
 * todo: merge with @see {@link AutomationActionItem}
 */
class WrappedAutomationConditionItem extends React.Component<Props, State> {

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
		const { fields } = this.state;

		if (this.props.condition?.type !== prevProps.condition?.type) {
			await this.fetchMetadata();
		}

		if (fields && this.props.dataFormChanges !== prevProps.dataFormChanges) {
			for (const field of this.props.dataFormChanges.changedFields) {
				const joinedName = joinNamePath(field.name);
				if (joinedName.startsWith(this.props.id)) {
					const dataField = fields.find(x => joinedName.endsWith("_" + x.key));
					if (dataField?.reloadMetadataOnChange) {
						await this.fetchMetadata();
					}
				}
			}
		}
	};

	fetchMetadata = async (): Promise<void> => {
		const { data, condition } = this.props;

		if (condition?.type) {
			const fields = await this.automationService.conditionMetadata(data.entityTypeCode, condition);

			this.setState({ loading: false, fields });
		} else {
			this.setState({ loading: false });
		}
	};

	render = () => {
		const { typeSelector, item, options } = this.props,
			{ fields } = this.state;

		const innerOptions: DataFormOptions = {
			// todo: return valid names with metadata
			namePathPrefix: extendNamePath(item.name, ["props"]),
			...options
		};

		return (<>
			<Space>
				{typeSelector}
			</Space>

			{fields && fields.map((field) => {
				const factory = DataFieldFactory.get(field.type);

				return factory?.createFormItem(field, null, innerOptions);
			})}

		</>);
	};
}

export const AutomationConditionItem = withAutomationContext(WrappedAutomationConditionItem);
