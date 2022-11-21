import { PageHeader } from "@ant-design/pro-components";
import { PageContextProps, StatusTag, withPageContext } from "@montr-core/components";
import { DataSider } from "@montr-core/components/data-sider";
import { DataTabs } from "@montr-core/components/data-tabs";
import { DataToolbar } from "@montr-core/components/data-toolbar";
import { withNavigate, withParams } from "@montr-core/components/react-router-wrappers";
import { ConfigurationItemProps, DataPaneProps, DataView } from "@montr-core/models";
import { Layout, Spin } from "antd";
import { Location } from "history";
import React from "react";
import { Navigate, NavigateFunction } from "react-router-dom";
import { Task } from "../models";
import { EntityTypeCode, RouteBuilder, Views } from "../module";
import { TaskService } from "../services/task-service";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props extends PageContextProps {
	params: RouteProps;
	navigate: NavigateFunction;
}

interface State {
	loading: boolean;
	modalOpen: boolean;
	confirmedNavigation: boolean;
	nextLocation?: Location;
	task?: Task;
	dataView?: DataView<Task>;
}

class PageViewTask extends React.Component<Props, State> {

	private readonly taskService = new TaskService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
			modalOpen: false,
			confirmedNavigation: false,
			task: {}
		};
	}

	getRouteProps = (): RouteProps => {
		return this.props.params;
	};

	componentDidMount = async (): Promise<void> => {
		const { addPageEventListener } = this.props;

		addPageEventListener(this);

		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		const { removePageEventListener } = this.props;

		removePageEventListener(this);

		await this.taskService.abort();
	};

	onPageSubmit = async (): Promise<void> => {
		return;
	};

	onPageSubmitted = async (): Promise<void> => {
		await this.fetchData();
	};

	onPageCancel = async (): Promise<void> => {
		return;
	};

	onModalConfirm = () => {
		const { setEditMode } = this.props;

		setEditMode(false);

		this.setState({ modalOpen: false, confirmedNavigation: true });
	};

	onModalClose = () => {
		this.setState({ modalOpen: false });
	};

	fetchData = async (): Promise<void> => {
		const { uid } = this.getRouteProps();

		const task = await this.taskService.get(uid);

		const dataView = await this.taskService.metadata(Views.taskPage, uid);

		this.setState({ loading: false, task, dataView });
	};

	handleDataChange = async (): Promise<void> => {
		await this.fetchData();
	};

	handleTabChange = (tabKey: string): void => {
		const { uid } = this.getRouteProps();

		const path = RouteBuilder.viewTask(uid, tabKey);

		this.props.navigate(path);
	};

	// https://v5.reactrouter.com/core/api/Prompt/message-func
	handleNavigation = (location: Location): string | boolean => {
		const { isDirty, setEditMode } = this.props,
			{ confirmedNavigation } = this.state;

		if (isDirty && !confirmedNavigation) {
			this.setState({ modalOpen: true, nextLocation: location });
			return false;
		} else {
			setEditMode(false);
			return true;
		}
	};

	render = (): React.ReactNode => {
		const { tabKey } = this.getRouteProps(),
			{ loading, modalOpen, confirmedNavigation, nextLocation, task = {}, dataView } = this.state;

		if (confirmedNavigation && nextLocation) {
			this.setState({ confirmedNavigation: null, nextLocation: null });
			return <Navigate to={nextLocation.pathname} replace={true} />;
		}

		if (!task) return null;

		const buttonProps: ConfigurationItemProps = {
			onDataChange: this.handleDataChange,
		};

		const paneProps: DataPaneProps<Task> = {
			task,
			entityTypeCode: EntityTypeCode.task,
			entityUid: task.uid
		};

		return (
			<Spin spinning={loading}>

				{/* https://reactrouter.com/docs/en/v6/upgrading/v5#prompt-is-not-currently-supported */}
				{/* <Prompt message={this.handleNavigation} /> */}

				{/* <Modal
					open={modalOpen}
					onOk={this.onModalConfirm}
					onCancel={this.onModalClose}>Are you sure you want to ...</Modal> */}

				<PageHeader
					onBack={() => window.history.back()}
					title={task.name}
					subTitle={<>{task.uid}</>}
					tags={<StatusTag statusCode={task.statusCode} />}
					// breadcrumb={<TaskBreadcrumb />}
					extra={<DataToolbar buttons={dataView?.toolbar} buttonProps={buttonProps} />}
				>
					{/* <TaskSignificantInfo task={task} /> */}
				</PageHeader>

				<Layout className="ant-page-layout">
					<Layout.Content>
						<DataTabs
							tabKey={tabKey}
							panes={dataView?.panes}
							onTabChange={this.handleTabChange}
							disabled={(_, index) => index > 0 && !task.uid}
							paneProps={paneProps}
						/>
					</Layout.Content>
					<Layout.Sider width={300} className="ant-page-layout-sider">
						<DataSider
							panes={dataView?.panels}
							paneProps={paneProps}
						/>
					</Layout.Sider>
				</Layout>

			</Spin>
		);
	};
}

export default withPageContext(withNavigate(withParams(PageViewTask)));
