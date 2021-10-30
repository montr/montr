import { AutomationConditionFactory, AutomationItemProps } from "@montr-automate/components";
import { AutomationService } from "@montr-automate/services";
import { DataFieldFactory, DataFormOptions } from "@montr-core/components";
import { IDataField } from "@montr-core/models";
import { Space } from "antd";
import React from "react";
import { CreateTaskAutomationAction } from "../models";

interface Props extends AutomationItemProps {
    action: CreateTaskAutomationAction;
}

interface State {
    loading: boolean;
    fields?: IDataField[];
}

export class CreateTaskAutomationActionItem extends React.Component<Props, State> {

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

    fetchMetadata = async (): Promise<void> => {
        const { action } = this.props;

        const fields = await this.automationService.metadata(action.type);

        this.setState({ loading: false, fields });
    };

    render = () => {
        const { typeSelector, item, options } = this.props,
            { fields } = this.state;

        const innerOptions: DataFormOptions = { namePathPrefix: [item.name, "props"], ...options };

        return (<>
            <Space align="start">
                {typeSelector}
            </Space>

            {fields && fields.map(field => {
                const factory = DataFieldFactory.get(field.type);

                return factory?.createFormItem(field, null, innerOptions);
            })}
        </>);
    };
}

export class CreateTaskAutomationActionFactory extends AutomationConditionFactory<CreateTaskAutomationAction> {
    createFormItem(action: CreateTaskAutomationAction, props: AutomationItemProps): React.ReactElement {
        return <CreateTaskAutomationActionItem action={action} {...props} />;
    }
}
