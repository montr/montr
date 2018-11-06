import * as React from "react";
import { Redirect } from 'react-router-dom'

import { Form, Input, Tooltip, Icon, Cascader, Select, Row, Col, Checkbox, Button, AutoComplete, message } from 'antd';
import { FormComponentProps } from 'antd/lib/form';

import { Event, EventAPI } from '../../api';
import { Page } from '../../components/';

interface EventFormProps extends FormComponentProps {
    name: string;
    description?: string;
}

interface EventFormState {
    toSearch: boolean   
}

class EventForm extends React.Component<EventFormProps, EventFormState> {
    constructor(props: EventFormProps) {
        super(props);

        this.state = { toSearch: false };
    }

    handleSubmit = (e: any) => {
        e.preventDefault();

        this.props.form.validateFieldsAndScroll((errors, values: Event) => {
            if (!errors) {
                EventAPI
                    .create(values)
                    .then(() => this.setState({ toSearch: true }));
            }
            else {
                message.error("Received errors: " + JSON.stringify(errors));
            }
        });
    }

    render() {

        if (this.state.toSearch == true) {
            return <Redirect to='/events' />
        }

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
                        initialValue: this.props.name
                    })(
                        <Input />
                    )}
                </Form.Item>
                <Form.Item {...formItemLayout} label="Описание" extra="Как можно подробнее опишите что вы хотите купить.">
                    {getFieldDecorator("description", {
                        initialValue: this.props.description
                    })(
                        <Input.TextArea rows={5} />
                    )}
                </Form.Item>
                <Form.Item {...tailFormItemLayout}>
                    <Button type="primary" htmlType="submit" icon="check">Сохранить</Button>
                </Form.Item>
            </Form >
        );
    }
}

const WrappedForm = Form.create<EventFormProps>()(EventForm);

export class CreateEvent extends React.Component {

    render() {
        const data = {
            name: "Новая процедура",
        }

        return (
            <Page title="Новая процедура">
                <WrappedForm {...data} />
            </Page>
        );
    }
}