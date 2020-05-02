import * as React from "react";
import { Form, Input, Checkbox, Button, Radio, Modal, message, Spin } from "antd";
import { CompanyService, CompanyMetadataService } from "../services";
import { ICompany } from "../models";
import { RadioChangeEvent } from "antd/lib/radio/interface";
import { NavigationService } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { withCompanyContext, CompanyContextProps } from ".";
import { FormInstance } from "antd/lib/form";
import { DataForm } from "@montr-core/components";
import { IDataField, IApiResult } from "@montr-core/models";


interface IProps extends CompanyContextProps {
}

interface IState {
	configCode?: string;
	modalVisible: boolean;
}

class _RegistrationForm extends React.Component<IProps, IState> {

	private _navigation = new NavigationService();
	private _companyService = new CompanyService();
	private _formRef = React.createRef<FormInstance>();

	constructor(props: IProps) {
		super(props);

		this.state = {
			configCode: "company",
			modalVisible: false
		};
	}

	componentWillUnmount = async () => {
		await this._companyService.abort();
	};

	handleSubmit = async (values: ICompany) => {
		const { manageCompany, switchCompany } = this.props;

		const result = await this._companyService.create({ item: values });

		if (result.success) {
			await switchCompany(result.uid);

			message.info(`Организация успешно зарегистрирована.`);

			const returnUrl = this._navigation.getUrlParameter(Constants.returnUrlParam);

			if (returnUrl) {
				this._navigation.navigate(returnUrl);
			}
			else {
				manageCompany();
			}
		}
	};

	// todo: use types
	checkIsChecked = (rule: any, value: any, callback: any) => {
		if (value === false) {
			callback("Вы должны согласиться с Условиями использования.");
		} else {
			callback();
		}
	};

	clearFormErrors = () => {
		const values = this._formRef.current.getFieldsValue();
		this._formRef.current.resetFields();
		this._formRef.current.setFieldsValue(values);
	};

	onChange = (e: RadioChangeEvent) => {
		this.setState({
			configCode: e.target.value as string
		});
		this.clearFormErrors();
	};

	showModal = (e: React.MouseEvent) => {
		e.preventDefault();

		this.setState({
			modalVisible: true,
		});
	};

	handleModalCancel = () => {
		this.setState({
			modalVisible: false
		});
	};

	render() {

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

		return <>
			<Form onFinish={this.handleSubmit} style={{ maxWidth: 600 }} initialValues={{ configCode, agreement: false }}>
				<Form.Item name="configCode" rules={[{ required: true }]} {...tailFormItemLayout}>
					<Radio.Group buttonStyle="solid" onChange={this.onChange}>
						<Radio.Button value="company">Организация</Radio.Button>
						<Radio.Button value="person">Физическое лицо</Radio.Button>
					</Radio.Group>
				</Form.Item>
				<Form.Item name="name"{...formItemLayout}
					rules={[{ required: true, whitespace: true, message: nameRequiredMessage }]}
					label={nameLabel}>
					<Input />
				</Form.Item>
				<Form.Item name="agreement" valuePropName="checked" {...tailFormItemLayout}
					rules={[{ required: true, message: fieldRequiredMessage }, {
						validator: this.checkIsChecked,
					}]}>
					<Checkbox>Прочитал и согласен с <a onClick={this.showModal}>Условиями использования</a></Checkbox>
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
		</>;
	}
}

const RegistrationForm = withCompanyContext(_RegistrationForm);

interface IRProps extends CompanyContextProps {
}

interface IRState {
	loading: boolean;
	fields?: IDataField[];
	data: any;
}

class _PageCompanyRegistration extends React.Component<IRProps, IRState> {

	private _navigation = new NavigationService();
	private _companyService = new CompanyService();
	private _metadataService = new CompanyMetadataService();

	constructor(props: IRProps) {
		super(props);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._companyService.abort();
		await this._metadataService.abort();
	};

	fetchData = async () => {
		const dataView = await this._metadataService.load();

		this.setState({ loading: false, data: {}, fields: dataView.fields });
	};

	handleSubmit = async (values: ICompany): Promise<IApiResult> => {
		const { manageCompany, switchCompany } = this.props;

		const result = await this._companyService.create({ item: values });

		if (result?.success) {
			await switchCompany(result.uid);

			if (!result.redirectUrl) {
				result.redirectUrl = this._navigation.getUrlParameter(Constants.returnUrlParam);
			}
		}

		return result;
	};

	render() {
		const { loading, fields, data } = this.state;

		return (
			<div>
				<h2>Регистрация</h2>

				<Spin spinning={loading} >
					<DataForm
						fields={fields}
						data={data}
						onSubmit={this.handleSubmit}
					/>
				</Spin>

				<RegistrationForm />

			</div>
		);
	}
}

export default withCompanyContext(_PageCompanyRegistration);
