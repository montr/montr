import { AutomationCondition } from "@montr-automate/models/automation";
import { DataFieldFactory, DataFormOptions } from "@montr-core/components";
import { IDataField } from "@montr-core/models";
import { Space } from "antd";
import React from "react";
import { AutomationService } from "../services";
import { AutomationItemProps } from "./automation-field-factory";

interface Props extends AutomationItemProps {
	condition: AutomationCondition;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}


/**
 * todo: merge with @see {@link AutomationActionItem}
 */
export class AutomationConditionItem extends React.Component<Props, State> {

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
		if (this.props.condition !== prevProps.condition) {
			await this.fetchMetadata();
		}
	};

	fetchMetadata = async (): Promise<void> => {
		const { condition } = this.props;

		if (condition?.type) {
			const fields = await this.automationService.metadata(null, condition.type);

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
			<Space align="start">
				{typeSelector}
			</Space>

			{fields && fields.map((field) => {
				const factory = DataFieldFactory.get(field.type);

				return factory?.createFormItem(field, null, innerOptions);
			})}
		</>);
	};
}
