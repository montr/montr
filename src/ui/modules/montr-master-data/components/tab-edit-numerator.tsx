import React from "react";
import { IDataField, IApiResult } from "@montr-core/models";
import { INumerator } from "../models";
import { NumeratorService } from "../services";
import { Spin } from "antd";
import { DataForm } from "@montr-core/components";
import { MetadataService } from "@montr-core/services";
import { Views } from "../module";

interface IProps {
	data: INumerator;
	onDataChange?: (values: INumerator) => void;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
}

export class TabEditNumerator extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _numeratorService = new NumeratorService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._numeratorService.abort();
	};

	fetchData = async () => {
		const dataView = await this._metadataService.load(Views.formEditNumerator);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: INumerator): Promise<IApiResult> => {
		const { data, onDataChange } = this.props;

		if (data.uid) {
			const updated = { uid: data.uid, ...values };

			const result = await this._numeratorService.update(updated);

			if (result.success) {
				if (onDataChange) await onDataChange(updated);
			}

			return result;
		}
		else {
			const result = await this._numeratorService.insert({ item: values });

			if (result.success) {
				if (onDataChange) await onDataChange(result);
			}

			return result;
		}
	};

	render = () => {
		const { data } = this.props,
			{ fields, loading } = this.state;

		return (
			<Spin spinning={loading}>
				{data && <DataForm fields={fields} data={data} onSubmit={this.save} />}
			</Spin>
		);
	};
}
