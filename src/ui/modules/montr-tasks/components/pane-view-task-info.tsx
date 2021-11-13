import { DataForm, PageContextProps, withPageContext } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { Spin } from "antd";
import React from "react";
import { Task } from "../models";
import { Api, Views } from "../module";
import { TaskService } from "../services";

interface Props extends PageContextProps {
    task: Task;
    mode?: "edit" | "view";
}

interface State {
    loading: boolean;
    fields?: IDataField[];
}

class PaneViewTaskInfo extends React.Component<Props, State> {

    private readonly taskService = new TaskService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true
        };
    }

    componentDidMount = async (): Promise<void> => {
        await this.fetchData();
    };

    componentDidUpdate = async (prevProps: Props): Promise<void> => {
        if (this.props.task !== prevProps.task) {
            await this.fetchData();
        }
    };

    componentWillUnmount = async (): Promise<void> => {
        await this.taskService.abort();
    };

    fetchData = async (): Promise<void> => {
        const { task } = this.props;

        const dataView = (task.uid)
            ? await this.taskService.metadata(Views.taskForm, task.uid)
            : null;

        this.setState({ loading: false, fields: dataView?.fields });
    };

    handleSubmit = async (values: Task): Promise<ApiResult> => {
        const { task } = this.props;

        return await this.taskService.post(Api.taskUpdate, {
            item: { uid: task.uid, ...values }
        });
    };

    render = (): React.ReactNode => {
        const { task, mode = "view", isEditMode } = this.props,
            { fields, loading } = this.state;

        return (
            <Spin spinning={loading}>
                <DataForm
                    mode={isEditMode ? "edit" : "view"}
                    fields={fields}
                    data={task}
                    onSubmit={this.handleSubmit} />
            </Spin>
        );
    };
}

const PaneViewTaskInfoWrapper = withPageContext(PaneViewTaskInfo);

export default PaneViewTaskInfoWrapper; 
