import * as React from "react";

import { Guid } from "@montr-core/.";

import { Form, Input, Checkbox, Button } from "antd";
import { FormComponentProps } from "antd/lib/form";

import { AuthService } from "@montr-core/services/AuthService";
import { CompanyAPI } from "../../api/CompanyAPI";

interface IProps extends FormComponentProps {
}

interface IState {
}

class RegistrationForm extends React.Component<IProps, IState> {

	public authService: AuthService;

	constructor(props: IProps) {
		super(props);

		this.state = {
		};

		this.authService = new AuthService();
	}

	handleSubmit = (e: React.SyntheticEvent) => {
		e.preventDefault();
		this.props.form.validateFieldsAndScroll((err, values) => {
			if (!err) {
				console.log("Received values of form: ", values);

				this.authService
					.getUser()
					.then((user: any) => {

						if (user && user.access_token) {
							CompanyAPI
								.create(Object.assign(values, { token: user.access_token }))
								.then((uid: Guid) => {
									console.log(uid);
								});
						}

					});
			}
		});
	}

	// todo: use types
	checkIsChecked = (rule: any, value: any, callback: any) => {
		if (value === false) {
			callback("You should agree with Terms of Service!");
		} else {
			callback();
		}
	}

	render() {
		const { getFieldDecorator } = this.props.form;

		const formItemLayout = {
			labelCol: {
				xs: { span: 24 },
				sm: { span: 8 },
			},
			wrapperCol: {
				xs: { span: 24 },
				sm: { span: 16 },
			},
		};
		const tailFormItemLayout = {
			wrapperCol: {
				xs: {
					span: 24,
					offset: 0,
				},
				sm: {
					span: 16,
					offset: 8,
				},
			},
		};

		return (
			<Form onSubmit={this.handleSubmit} style={{ maxWidth: 600 }}>
				<Form.Item
					{...formItemLayout}
					label="Наименование компании">
					{getFieldDecorator("name", {
						rules: [{ required: true, whitespace: true }],
					})(
						<Input />
					)}
				</Form.Item>
				<Form.Item {...tailFormItemLayout}>
					{getFieldDecorator("agreement", {
						rules: [{ required: true }, {
							validator: this.checkIsChecked,
						}],
						valuePropName: "checked",
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

const WrappedRegistrationForm = Form.create()(RegistrationForm);

export class Registration extends React.Component {
	render() {
		return (
			<div>
				<h1>Регистрация компании</h1>
				<WrappedRegistrationForm />
			</div>
		);
	}
}
