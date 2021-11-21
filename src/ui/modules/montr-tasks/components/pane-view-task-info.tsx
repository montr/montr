import { DataForm, PageContextProps, PageEventListener, withPageContext } from "@montr-core/components";
import { ApiResult, DataPaneProps, IDataField } from "@montr-core/models";
import { FormInstance, Spin } from "antd";
import React from "react";
import { Task } from "../models";
import { Api, Views } from "../module";
import { TaskService } from "../services";

interface Props extends DataPaneProps<Task>, PageContextProps {
    mode?: "edit" | "view";
}

interface State {
    loading: boolean;
    data?: Task;
    fields?: IDataField[];
}

class PaneViewTaskInfo extends React.Component<Props, State> implements PageEventListener {

    private readonly formRef = React.createRef<FormInstance>();
    private readonly taskService = new TaskService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true
        };
    }

    componentDidMount = async (): Promise<void> => {
        const { addPageEventListener } = this.props;

        addPageEventListener(this);

        await this.fetchData();
    };

    componentDidUpdate = async (prevProps: Props): Promise<void> => {
        if (this.props.task !== prevProps.task) {
            await this.fetchData();
        }
    };

    componentWillUnmount = async (): Promise<void> => {
        const { removePageEventListener } = this.props;

        removePageEventListener(this);

        await this.taskService.abort();
    };

    onPageSubmit = async (): Promise<void> => {
        await this.formRef.current.submit();
    };

    onPageSubmitted = async (): Promise<void> => {
        return;
    };

    onPageCancel = async (): Promise<void> => {
        this.formRef.current.resetFields();
    };

    fetchData = async (): Promise<void> => {
        const { entityUid } = this.props;

        const data = (entityUid)
            ? await this.taskService.get(entityUid)
            : null;

        const dataView = (entityUid)
            ? await this.taskService.metadata(Views.taskForm, entityUid)
            : null;

        this.setState({ loading: false, data, fields: dataView?.fields });
    };

    handleSubmit = async (values: Task): Promise<ApiResult> => {
        const { entityUid, setEditMode, onPageSubmitted } = this.props;

        // todo: move to task service
        const result: ApiResult = await this.taskService.post(Api.taskUpdate, {
            item: { uid: entityUid, ...values }
        });

        if (result.success) {
            await this.fetchData();

            onPageSubmitted();
            setEditMode(false); // todo: should be called after all submits
        }

        return result;
    };

    render = (): React.ReactNode => {
        const { mode = "view", isEditMode } = this.props,
            { loading, data, fields } = this.state;

        return (
            <Spin spinning={loading}>
                <DataForm
                    formRef={this.formRef}
                    mode={isEditMode ? "edit" : "view"} // todo: use mode from props too
                    fields={fields}
                    data={data}
                    onSubmit={this.handleSubmit}
                    hideButtons={true}
                />
            </Spin>
        );
    };
}

export default withPageContext(PaneViewTaskInfo); 
