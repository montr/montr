import * as React from "react";

import { Form, Input, Button, message } from "antd";
import { FormComponentProps } from "antd/lib/form";

import { IEvent, EventAPI, IApiResult, IEventTemplate, IPaneProps } from "../../api";
import { IPaneComponent } from "./EditEvent";

interface IEventFormProps extends FormComponentProps {
    data: IEvent;
}

interface IEventFormState {
}

class EventForm extends React.Component<IEventFormProps, IEventFormState> {
    constructor(props: IEventFormProps) {
        super(props);
        this.state = {};
    }

    handleSubmit = (e: React.SyntheticEvent) => {
        e.preventDefault();
        this.save();
    }

    save() {
        this.props.form.validateFieldsAndScroll((errors, values: IEvent) => {
            if (!errors) {
                EventAPI
                    .update({ id: this.props.data.id, ...values })
                    .then((result: IApiResult) => {
                        message.success("Данные успешно сохранены");
                    });
            }
            else {
                message.error("Received errors: " + JSON.stringify(errors));
            }
        });
    }

    render() {

        const { getFieldDecorator } = this.props.form;

        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 8 },
                lg: { span: 4 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 16 },
                lg: { span: 20 },
            },
        };

        const tailFormItemLayout = {
            wrapperCol: {
                xs: { offset: 0, span: 24, },
                sm: { offset: 8, span: 16, },
                lg: { offset: 4, span: 20, },
            },
        };

        return (
            <Form onSubmit={this.handleSubmit}>
                <Form.Item {...formItemLayout} label="Наименование">
                    {getFieldDecorator("name", {
                        rules: [
                            { required: true, whitespace: true, message: "Поле «Наименование» обязательно для заполнения" }
                        ],
                        initialValue: this.props.data.name
                    })(
                        <Input.TextArea autosize={{ minRows: 2, maxRows: 7 }} />
                    )}
                </Form.Item>
                <Form.Item {...formItemLayout} label="Описание" extra="Как можно подробнее опишите что вы хотите купить.">
                    {getFieldDecorator("description", {
                        initialValue: this.props.data.description
                    })(
                        <Input.TextArea autosize={{ minRows: 4, maxRows: 10 }} />
                    )}
                </Form.Item>
               {/*  <Form.Item {...tailFormItemLayout}>
                    <Button type="primary" htmlType="submit" icon="check">Сохранить</Button>
                </Form.Item> */}
            </Form >
        );
    }
}

const WrappedForm = Form.create<IEventFormProps>()(EventForm);

interface IEditEventPaneProps extends IPaneProps<IEvent> {
    data: IEvent;
}

interface IEditEventTabState {
}

export class EditEventPane extends React.Component<IEditEventPaneProps, IEditEventTabState> {

    private _formRef: IPaneComponent;

    save() {
        this._formRef.save();
    }

    render() {
        return (
            <WrappedForm data={this.props.data} wrappedComponentRef={(form: any) => this._formRef = form} />
        );
    }
}