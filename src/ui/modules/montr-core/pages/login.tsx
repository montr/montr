import * as React from "react";
import { Page, DataForm } from "../components";
import { IIndexer, IFormField, IApiResult } from "@montr-core/models";
import { Spin } from "antd";
import { MetadataService } from "@montr-core/services";
import { ILoginModel } from "@montr-core/models/login-model";

interface IProps {
}

interface IState {
	loading: boolean;
	data: ILoginModel;
	fields?: IFormField[];
}

export default class Login extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			data: {},
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		/* if (this.props.type !== prevProps.type ||
			this.props.currentCompany !== prevProps.currentCompany) {
			await this.fetchData();
		} */
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
	}

	fetchData = async () => {
		const dataView = await this._metadataService.load("Login/Form");

		this.setState({ loading: false, fields: dataView.fields });
	}

	save = async (values: ILoginModel): Promise<IApiResult> => {
		return null;
	}

	render = () => {
		const { fields, data, loading } = this.state;

		return (
			<Page title="Log in">

				<h3>Use a local account to log in.</h3>

				<div style={{ width: "33%" }} >
					<Spin spinning={loading}>
						<DataForm fields={fields} data={data} onSave={this.save} />
					</Spin>
				</div>

				<h3>Use another service to log in.</h3>

			</Page>
		);
	}
}
