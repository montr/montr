import { PageHeader } from "@ant-design/pro-components";
import { DataTable, DataTableUpdateToken } from "@montr-core/components/data-table";
import { DataView } from "@montr-core/models/data-view";
import { Task } from "@montr-tasks/models/task";
import { TaskMetadataService } from "@montr-tasks/services/task-metadata-service";
import { Spin } from "antd";
import React from "react";
import { Api } from "../module";

interface State {
	loading: boolean;
	dataView?: DataView<Task>;
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

export default class PageSearchTasks extends React.Component<unknown, State> {

	private readonly taskMetdataService = new TaskMetadataService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true,
			selectedRowKeys: [],
			updateTableToken: { date: new Date() }
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchMetadata();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.taskMetdataService.abort();
	};

	fetchMetadata = async (): Promise<void> => {
		const dataView = await this.taskMetdataService.searchMetadata();

		this.setState({ loading: false, dataView });
	};

	render = (): React.ReactNode => {

		const { loading, dataView, updateTableToken } = this.state;

		return (

			<Spin spinning={loading}>

				<PageHeader
					onBack={() => window.history.back()}
					title={"Tasks"}
				// subTitle={}
				// tags={}
				// breadcrumb={<TaskBreadcrumb />}
				//extra={<DataToolbar buttons={dataView.toolbar} buttonProps={buttonProps} />}
				>
					{/* <DocumentSignificantInfo document={document} /> */}
				</PageHeader>

				{/* <SearchCriteria
					fields={dataView?.fields}
				/> */}

				<DataTable
					rowKey="uid"
					columns={dataView?.columns}
					loadUrl={Api.taskList}
					updateToken={updateTableToken}
				/>

			</Spin>
		);
	};
}
