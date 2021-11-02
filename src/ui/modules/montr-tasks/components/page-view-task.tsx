import { DataTabs, StatusTag } from "@montr-core/components";
import { DataPaneProps, DataView, IIndexer } from "@montr-core/models";
import { PageHeader, Spin } from "antd";
import React from "react";
import { RouteComponentProps } from "react-router";
import { Task } from "../models";
import { EntityTypeCode, RouteBuilder, Views } from "../module";
import { TaskService } from "../services";

interface RouteProps {
    uid?: string;
    tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
    loading: boolean;
    task?: Task;
    dataView?: DataView<IIndexer>;
}

export default class PageViewTask extends React.Component<Props, State> {

    private readonly taskService = new TaskService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true,
            task: {}
        };
    }

    componentDidMount = async (): Promise<void> => {
        await this.fetchData();
    };

    componentWillUnmount = async (): Promise<void> => {
        await this.taskService.abort();
    };

    fetchData = async (): Promise<void> => {
        const { uid } = this.props.match.params;

        const task = await this.taskService.get(uid);

        const dataView = await this.taskService.metadata(Views.taskForm, task.uid);

        this.setState({ loading: false, task, dataView });
    };

    handleDataChange = async (): Promise<void> => {
        await this.fetchData();
    };

    handleTabChange = (tabKey: string): void => {
        const { uid } = this.props.match.params;

        const path = RouteBuilder.viewTask(uid, tabKey);

        this.props.history.replace(path);
    };

    render = (): React.ReactNode => {
        const { tabKey } = this.props.match.params,
            { loading, task = {}, dataView } = this.state;

        const paneProps: DataPaneProps<Task> = {
            task,
            entityTypeCode: EntityTypeCode.task,
            entityUid: task.uid
        };

        return (
            <Spin spinning={loading}>
                {task.uid && <>
                    <PageHeader
                        onBack={() => window.history.back()}
                        title={task.name}
                        subTitle={task.uid}
                        tags={<StatusTag statusCode={task.statusCode} />}
                    // breadcrumb={<DocumentBreadcrumb />}
                    // extra={<DataToolbar buttons={dataView.toolbar} buttonProps={buttonProps} />}
                    >
                        {/* <DocumentSignificantInfo document={document} /> */}
                    </PageHeader>

                    <DataTabs
                        tabKey={tabKey}
                        panes={dataView?.panes}
                        onTabChange={this.handleTabChange}
                        disabled={(_, index) => index > 0 && !task.uid}
                        tabProps={paneProps}
                    /></>}

            </Spin>
        );
    };
}
