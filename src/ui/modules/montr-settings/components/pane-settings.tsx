import { DataForm } from "@montr-core/components/data-form";
import { ApiResult } from "@montr-core/models/api-result";
import { Guid } from "@montr-core/models/guid";
import { SettingsBlock } from "@montr-settings/models/settings-block";
import { SettingsService } from "@montr-settings/services/settings-service";
import { Spin } from "antd";
import React from "react";

interface Props {
	entityTypeCode: string;
	entityUid: string | Guid;
	category: string;
}

interface State {
	loading: boolean;
	blocks?: SettingsBlock[];
}

export default class PaneSettings extends React.Component<Props, State> {

	private readonly settingsService = new SettingsService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchMetadata();
	};

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.category !== prevProps.category) {
			await this.fetchMetadata();
		}
	};

	fetchMetadata = async (): Promise<void> => {
		const { entityTypeCode, entityUid, category } = this.props;

		const blocks = await this.settingsService.metadata(entityTypeCode, entityUid, category);

		this.setState({ loading: false, blocks });
	};

	render = (): React.ReactNode => {
		const { entityTypeCode, entityUid } = this.props,
			{ loading, blocks } = this.state;

		return (
			<Spin spinning={loading}>

				{blocks?.map(block =>
					<SettingsForm
						key={block?.typeCode}
						entityTypeCode={entityTypeCode}
						entityUid={entityUid}
						block={block}
					/>
				)}

			</Spin>
		);
	};
}

interface SettingsFormProps {
	entityTypeCode: string;
	entityUid: string | Guid;
	block: SettingsBlock;
}

interface SettingsFormState {
	loading: boolean;
	data?: unknown;
}

class SettingsForm extends React.Component<SettingsFormProps, SettingsFormState> {

	private readonly settingsService = new SettingsService();

	constructor(props: SettingsFormProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	fetchData = async (): Promise<void> => {

		const { entityTypeCode, entityUid, block } = this.props;

		const data = await this.settingsService.get(entityTypeCode, entityUid, block.typeCode);

		this.setState({ loading: false, data: data?.data || {} });
	};

	handleSubmit = async (values: unknown): Promise<ApiResult> => {
		const { entityTypeCode, entityUid, block } = this.props;

		const result: ApiResult = await this.settingsService
			.update(entityTypeCode, entityUid, block.typeCode, values);

		if (result.success) {
			await this.fetchData();
		}

		return result;
	};

	render = (): React.ReactNode => {
		const { block } = this.props,
			{ loading, data } = this.state;

		return (
			<Spin spinning={loading}>
				<h3>{block?.displayName}</h3>

				{data && <DataForm
					fields={block?.fields}
					data={data}
					onSubmit={this.handleSubmit}
				/>}
			</Spin>
		);
	};
}
