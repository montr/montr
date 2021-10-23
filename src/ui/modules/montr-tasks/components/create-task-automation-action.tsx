import { AutomationConditionFactory, AutomationItemProps } from "@montr-automate/components";
import { FormDefaults } from "@montr-core/components";
import { Form, Input, Space } from "antd";
import React from "react";
import { CreateTaskAutomationAction } from "../models";

interface Props extends AutomationItemProps {
    action: CreateTaskAutomationAction;
}

export class CreateTaskAutomationActionItem extends React.Component<Props> {

    render = () => {
        const { typeSelector, item } = this.props;

        const { key, ...other } = item;
        const itemProps = { /* ...other, */ ...FormDefaults.formItemLayout };

        return (
            <>
                <Space align="start">
                    {typeSelector}
                </Space>

                <Form.Item
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
                </Form.Item>
            </>
        );
    };
}

export class CreateTaskAutomationActionFactory extends AutomationConditionFactory<CreateTaskAutomationAction> {
    createFormItem(action: CreateTaskAutomationAction, props: AutomationItemProps): React.ReactElement {
        return <CreateTaskAutomationActionItem action={action} {...props} />;
    }
}
