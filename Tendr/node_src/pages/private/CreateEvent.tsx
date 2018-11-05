import * as React from "react";

import { Form, Input, Tooltip, Icon, Cascader, Select, Row, Col, Checkbox, Button, AutoComplete } from 'antd';
import { FormComponentProps } from 'antd/lib/form';

import { Page } from '../../components/';

interface EventFormProps extends FormComponentProps {
    age: number;
    nickname: string;
    email: string;
}

class EventForm extends React.Component<EventFormProps, any> {

    handleSubmit = (e: any) => {
        e.preventDefault();
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
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
                xs: {
                    span: 24,
                    offset: 0,
                },
                sm: {
                    offset: 8,
                    span: 16,
                },
                lg: {
                    offset: 4,
                    span: 20,
                },
            },
        };

        return (
            <Form onSubmit={this.handleSubmit}>
                <Form.Item {...formItemLayout} label="E-mail">
                    {getFieldDecorator('email', {
                        rules: [
                            { type: 'email', message: 'The input is not valid E-mail!' },
                            { required: true, message: 'Please input your E-mail!' }
                        ],
                        initialValue: this.props.email
                    })(
                        <Input />
                    )}
                </Form.Item>
                <Form.Item {...formItemLayout} label={(
                    <span>
                        Nickname &nbsp;
                            <Tooltip title="What do you want others to call you?">
                            <Icon type="question-circle-o" />
                        </Tooltip>
                    </span>
                )}>
                    {getFieldDecorator('nickname', {
                        rules: [{ required: true, message: 'Please input your nickname!', whitespace: true }],
                        initialValue: this.props.nickname
                    })(
                        <Input />
                    )}
                </Form.Item>
                <Form.Item {...tailFormItemLayout}>
                    {getFieldDecorator('agreement', {
                        valuePropName: 'checked',
                    })(
                        <Checkbox>I have read the <a href="">agreement</a></Checkbox>
                    )}
                </Form.Item>
                <Form.Item {...tailFormItemLayout}>
                    <Button type="primary" htmlType="submit">Register</Button>
                </Form.Item>
            </Form>
        );
    }
}

const WrappedForm = Form.create<EventFormProps>()(EventForm);

export class CreateEvent extends React.Component {

    render() {
        const data = {
            age: 42,
            nickname: "vasya",
            email: "vpupkin@ya.ru"
        }

        return (
            <Page title="Create Event">
                <WrappedForm {...data} />
            </Page>
        );
    }
}