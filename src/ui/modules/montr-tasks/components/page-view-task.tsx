import { DataSider, DataTabs, DataToolbar, PageContextProps, StatusTag, withPageContext } from "@montr-core/components";
import { ConfigurationItemProps, DataPaneProps, DataView } from "@montr-core/models";
import { Layout, Modal, PageHeader, Spin } from "antd";
import { Location } from "history";
import React from "react";
import { Prompt, Redirect, RouteComponentProps } from "react-router";
import { Task } from "../models";
import { EntityTypeCode, RouteBuilder, Views } from "../module";
import { TaskService } from "../services";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps>, PageContextProps {
}

interface State {
	loading: boolean;
	modalVisible: boolean;
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
			modalVisible: false,
			confirmedNavigation: false,
			task: {}
		};
	}

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

		this.setState({ modalVisible: false, confirmedNavigation: true });
	};

	onModalClose = () => {
		this.setState({ modalVisible: false });
	};

	fetchData = async (): Promise<void> => {
		const { uid } = this.props.match.params;

		const task = await this.taskService.get(uid);

		const dataView = await this.taskService.metadata(Views.taskPage, task.uid);

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

	// https://v5.reactrouter.com/core/api/Prompt/message-func
	handleNavigation = (location: Location): string | boolean => {
		const { isDirty, setEditMode } = this.props,
			{ confirmedNavigation } = this.state;

		if (isDirty && !confirmedNavigation) {
			this.setState({ modalVisible: true, nextLocation: location });
			return false;
		} else {
			setEditMode(false);
			return true;
		}
	};

	render = (): React.ReactNode => {
		const { tabKey } = this.props.match.params,
			{ loading, modalVisible, confirmedNavigation, nextLocation, task = {}, dataView } = this.state;

		if (confirmedNavigation && nextLocation) {
			this.setState({ confirmedNavigation: null, nextLocation: null });
			return <Redirect to={nextLocation.pathname} push={true} />;
		}

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

				<Prompt message={this.handleNavigation} />

				<Modal
					visible={modalVisible}
					onOk={this.onModalConfirm}
					onCancel={this.onModalClose}>Are you sure you want to ...</Modal>

				<PageHeader
					onBack={() => window.history.back()}
					title={task.name}
					subTitle={task.uid}
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

export default withPageContext(PageViewTask);
