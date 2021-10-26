import { AutomationConditionFactory, AutomationItemProps } from "@montr-automate/components";
import { AutomationService } from "@montr-automate/services";
import { DataFieldFactory, FormDefaults } from "@montr-core/components";
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

        const { key, ...other } = item;
        const itemProps = { /* ...other, */ ...FormDefaults.formItemLayout };

        return (
            <>
                <Space align="start">
                    {typeSelector}
                </Space>

                {fields && fields.map(field => {
                    const factory = DataFieldFactory.get(field.type);

                    if (!factory) {
                        // todo: display default placeholder for not found field type
                        console.warn(`Warning: Field type '${field.type}' is not found.`);
                        return null;
                    }

                    const name = [item.name, "props", field.key];

                    return factory.createFormItem(field, null /* item */, options, name);
                })}

                {/* <Form.Item
                    {...itemProps}
                    label="Name"
                    name={[item.name, "props", "name"]}
                    fieldKey={[item.fieldKey, "name"]}
                    rules={[{ required: true }]}>
                    <Input placeholder="Name" />
                </Form.Item>

                <Form.Item
                    {...itemProps}
                    label="Description"
                    name={[item.name, "props", "description"]}
                    fieldKey={[item.fieldKey, "description"]}
                    rules={[{ required: false }]}>
                    <Input.TextArea placeholder="Description" rows={4} />
                </Form.Item> */}
            </>
        );
    };
}

export class CreateTaskAutomationActionFactory extends AutomationConditionFactory<CreateTaskAutomationAction> {
    createFormItem(action: CreateTaskAutomationAction, props: AutomationItemProps): React.ReactElement {
        return <CreateTaskAutomationActionItem action={action} {...props} />;
    }
}
