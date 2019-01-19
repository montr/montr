import * as React from "react";

import { Guid } from "@montr-core/.";

import { Form, Input, Checkbox, Button, Radio, Modal } from "antd";
import { FormComponentProps } from "antd/lib/form";

import { CompanyAPI } from "../../api/CompanyAPI";
import { RadioChangeEvent } from "antd/lib/radio/interface";

interface IProps extends FormComponentProps {
}

interface IState {
	configCode?: string;
	modalVisible: boolean;
}

class RegistrationForm extends React.Component<IProps, IState> {

	constructor(props: IProps) {
		super(props);

		this.state = {
			configCode: "company",
			modalVisible: false
		};
	}

	handleSubmit = (e: React.SyntheticEvent) => {
		e.preventDefault();
		this.props.form.validateFieldsAndScroll((err, values) => {
			if (!err) {
				console.log(values);

				CompanyAPI
					.create(values)
					.then((uid: Guid) => {
						console.log(uid);
					});
			}
		});
	}

	// todo: use types
	checkIsChecked = (rule: any, value: any, callback: any) => {
		if (value === false) {
			callback("Вы должны согласиться с Условиями использования.");
		} else {
			callback();
		}
	}

	clearFormErrors = () => {
		const values = this.props.form.getFieldsValue();
		this.props.form.resetFields();
		this.props.form.setFieldsValue(values);
	}

	onChange = (e: RadioChangeEvent) => {
		this.setState({
			configCode: e.target.value
		});
		this.clearFormErrors();
	}

	showModal = (e: React.MouseEvent) => {
		e.preventDefault();

		this.setState({
			modalVisible: true,
		});
	}

	handleModalCancel = () => {
		this.setState({
			modalVisible: false
		});
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

		const { configCode } = this.state;

		const nameLabel = (configCode == "company") ? "Наименование" : "Ф.И.О.",
			nameRequiredMessage = `Поле «${nameLabel}» обязательно для заполнения`,
			fieldRequiredMessage = `Поле обязательно для заполнения`;

		return (
			<>
				<Form onSubmit={this.handleSubmit} style={{ maxWidth: 600 }}>
					<Form.Item {...tailFormItemLayout}>
						{getFieldDecorator("config_code", {
							rules: [{ required: true }],
							initialValue: configCode
						})(
							<Radio.Group buttonStyle="solid" onChange={this.onChange}>
								<Radio.Button value="company">Организация</Radio.Button>
								<Radio.Button value="person">Физическое лицо</Radio.Button>
							</Radio.Group>
						)}
					</Form.Item>
					<Form.Item
						{...formItemLayout}
						label={nameLabel}>
						{getFieldDecorator("full_name", {
							rules: [{ required: true, whitespace: true, message: nameRequiredMessage }],
						})(
							<Input />
						)}
					</Form.Item>
					<Form.Item {...tailFormItemLayout}>
						{getFieldDecorator("agreement", {
							rules: [{ required: true, message: fieldRequiredMessage }, {
								validator: this.checkIsChecked,
							}],
							initialValue: false,
							valuePropName: "checked",
						})(
							<Checkbox>Прочитал и согласен с <a onClick={this.showModal}>Условиями использования</a></Checkbox>
						)}
					</Form.Item>
					<Form.Item {...tailFormItemLayout}>
						<Button type="primary" htmlType="submit">Зарегистрироваться</Button>
					</Form.Item>
				</Form>
				<Modal
					title="Условия использования" footer={null}
					onCancel={this.handleModalCancel}
					visible={this.state.modalVisible}>
					<p>Some contents...</p>
					<p>Some contents...</p>
					<p>Some contents...</p>
				</Modal>
			</>
		);
	}
}

const WrappedRegistrationForm = Form.create()(RegistrationForm);

export class Registration extends React.Component {
	render() {
		return (
			<div>
				<h1>Регистрация</h1>
				<WrappedRegistrationForm />
			</div>
		);
	}
}
