import { DataForm } from "@montr-core/components/data-form";
import { ApiResult } from "@montr-core/models/api-result";
import { IDataField } from "@montr-core/models/data-field";
import { Guid } from "@montr-core/models/guid";
import { SettingsService } from "@montr-settings/services/settings-service";
import { Divider, Spin } from "antd";
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

// todo: remove hardcoded value
const settingsTypeCode = "Montr.Messages.SmtpOptions";

export default class PaneSettings extends React.Component<Props, State> {

	private readonly settingsService = new SettingsService();

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

		const dataView = await this.settingsService
			.metadata(entityTypeCode, entityUid, settingsTypeCode);

		const data = await this.settingsService
			.get(entityTypeCode, entityUid, settingsTypeCode);

		this.setState({ loading: false, data: data?.data, fields: dataView?.fields });
	};

	handleSubmit = async (values: unknown): Promise<ApiResult> => {
		const { entityTypeCode, entityUid } = this.props;

		const result: ApiResult = await this.settingsService
			.update(entityTypeCode, entityUid, settingsTypeCode, values);

		if (result.success) {
			await this.fetchData();
		}

		return result;
	};

	render = (): React.ReactNode => {
		const { loading, data, fields } = this.state;

		return (
			<Spin spinning={loading}>
				<Divider orientation="left">{settingsTypeCode}</Divider>

				<DataForm
					fields={fields}
					data={data}
					onSubmit={this.handleSubmit}
				/>
			</Spin>
		);
	};
}
