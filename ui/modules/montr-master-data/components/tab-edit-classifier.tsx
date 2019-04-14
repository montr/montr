import * as React from "react";
import { Redirect } from "react-router";
import { Spin } from "antd";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { IFormField, Guid } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { DataForm } from "@montr-core/components";
import { ClassifierService } from "../services";
import { IClassifier, IClassifierType } from "../models";
import { RouteBuilder } from "..";

interface IProps extends CompanyContextProps {
	type: IClassifierType;
	data: IClassifier;
	onDataChange?: (values: IClassifier) => void
}

interface IState {
	loading: boolean;
	fields?: IFormField[];
	redirect?: string;
}

class _TabEditClassifier extends React.Component<IProps, IState> {
	private _metadataService = new MetadataService();
	private _classifierService = new ClassifierService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.type !== prevProps.type ||
			this.props.currentCompany !== prevProps.currentCompany) {
			await this.fetchData();
		}
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._classifierService.abort();
	}

	private fetchData = async () => {
		const { type, currentCompany } = this.props;

		if (type && currentCompany) {

			const dataView = await this._metadataService.load(`Classifier/${type.code}`);

			this.setState({ loading: false, fields: dataView.fields });
		}
	}

	private save = async (values: IClassifier) => {
		const { type, data, onDataChange } = this.props;
		const { uid: companyUid } = this.props.currentCompany;

		if (data.uid) {
			const updated = { uid: data.uid, ...values };
			const rowsUpdated = await this._classifierService.update(companyUid, updated);

			if (rowsUpdated != 1) throw new Error();

			if (onDataChange) await onDataChange(updated);
		}
		else {
			const uid: Guid = await this._classifierService.insert(companyUid, type.code, values);

			const path = RouteBuilder.editClassifier(type.code, uid);

			this.setState({ redirect: path });
		}
	}

	render = () => {
		const { data } = this.props,
			{ redirect, fields } = this.state;

		if (redirect) {
			this.setState({ redirect: null });
			return <Redirect to={redirect} />
		}

		return (
			<Spin spinning={this.state.loading}>
				<DataForm fields={fields} data={data} onSave={this.save} />
			</Spin>
		);
	}
}

export const TabEditClassifier = withCompanyContext(_TabEditClassifier);
