import { DataTable, DataTableUpdateToken } from "@montr-core/components";
import { DataColumn } from "@montr-core/models";
import { PageHeader, Spin } from "antd";
import React from "react";
import { Api, Views } from "../module";
import { TaskService } from "../services";

interface State {
    loading: boolean;
    columns?: DataColumn[];
    selectedRowKeys: string[] | number[];
    updateTableToken: DataTableUpdateToken;
}

export default class PageSearchTasks extends React.Component<unknown, State> {

    private readonly taskService = new TaskService();

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
        await this.taskService.abort();
    };

    fetchMetadata = async (): Promise<void> => {
        const dataView = await this.taskService.metadata(Views.taskList);

        this.setState({ loading: false, columns: dataView.columns });
    };

    render = (): React.ReactNode => {

        const { loading, columns, updateTableToken } = this.state;

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

                <DataTable
                    rowKey="uid"
                    columns={columns}
                    loadUrl={Api.taskList}
                    updateToken={updateTableToken}
                />

            </Spin>
        );
    };
}
