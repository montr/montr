import { DataForm } from "@montr-core/components/data-form";
import { IDataField } from "@montr-core/models/data-field";
import { Guid } from "@montr-core/models/guid";
import { SettingsService } from "@montr-settings/services/settings-service";
import { Spin } from "antd";
import React from "react";

interface Props {
	entityTypeCode: string;
	entityUid: Guid;
}

interface State {
	loading: boolean;
	data?: unknown;
	fields?: IDataField[];
}

export default class PaneSettings extends React.Component<Props, State> {

	private readonly taskService = new SettingsService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	fetchData = async (): Promise<void> => {

		const { entityTypeCode, entityUid } = this.props;

		const dataView = await this.taskService.metadata(entityTypeCode, entityUid);

		this.setState({ loading: false, fields: dataView?.fields });
	};

	render = (): React.ReactNode => {
		const { loading, data = {}, fields } = this.state;

		return (
			<Spin spinning={loading}>
				<DataForm
					fields={fields}
					data={data}
				/>
			</Spin>
		);
	};
}
