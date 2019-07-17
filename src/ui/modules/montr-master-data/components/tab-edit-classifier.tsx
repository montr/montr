import * as React from "react";
import { Redirect } from "react-router";
import { Spin } from "antd";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { IFormField, IApiResult } from "@montr-core/models";
import { DataForm } from "@montr-core/components";
import { ClassifierService, ClassifierMetadataService } from "../services";
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
	private _classifierMetadataService = new ClassifierMetadataService();
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
		await this._classifierMetadataService.abort();
		await this._classifierService.abort();
	}

	fetchData = async () => {
		const { type, currentCompany } = this.props;

		if (type && currentCompany) {

			const dataView = await this._classifierMetadataService.load(currentCompany.uid, type.code);

			/* const fields = dataView.fields;

			const parentUidField = fields.find(x => x.key == "parentUid") as IClassifierField;

			if (parentUidField) {
				parentUidField.typeCode = type.code;
				// parentUidField.treeUid = treeUid;
			} */

			this.setState({ loading: false, fields: dataView.fields });
		}
	}

	save = async (values: IClassifier): Promise<IApiResult> => {
		const { type, data, onDataChange } = this.props;
		const { uid: companyUid } = this.props.currentCompany;

		if (data.uid) {
			const updated = { uid: data.uid, ...values };

			const result = await this._classifierService.update(companyUid, type.code, updated);

			if (result.success) {
				if (onDataChange) await onDataChange(updated);
			}

			return result;
		}
		else {
			const result = await this._classifierService.insert(companyUid, { typeCode: type.code, item: values });

			if (result.success) {
				this.setState({ redirect: RouteBuilder.editClassifier(type.code, result.uid) });
			}

			return result;
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
