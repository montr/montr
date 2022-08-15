import { DataBreadcrumb, DataTabs, Page, PageHeader, PaneComponent, Toolbar } from "@montr-core/components";
import { withNavigate, withParams } from "@montr-core/components/react-router-wrappers";
import { DataPaneProps, DataView } from "@montr-core/models";
import { MetadataService, OperationService } from "@montr-core/services";
import { Button, Spin, Tag } from "antd";
import i18next from "i18next";
import * as React from "react";
import { NavigateFunction, useMatch } from "react-router-dom";
import { IEvent } from "../models";
import { EntityTypeCode, Locale, Patterns, RouteBuilder } from "../module";
import { EventService, EventTemplateService } from "../services";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props {
	params: RouteProps;
	navigate: NavigateFunction;
}

interface State {
	loading: boolean;
	data: IEvent;
	dataView?: DataView<IEvent>;
	configCodes: IEvent[];
}

class PageEditEvent extends React.Component<Props, State> {

	private readonly operation = new OperationService();
	private readonly metadataService = new MetadataService();
	private readonly eventTemplateService = new EventTemplateService();
	private readonly eventService = new EventService();

	private _refsByKey: Map<string, any> = new Map<string, React.RefObject<any>>();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
			configCodes: [],
			data: {}
		};
	}

	getRouteProps = (): RouteProps => {
		return this.props.params;
	};

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.metadataService.abort();
		await this.eventTemplateService.abort();
		await this.eventService.abort();
	};

	fetchData = async (): Promise<void> => {
		const templates = await this.eventTemplateService.list();

		// todo: get metadata key from server
		const dataView = await this.metadataService.load("PrivateEvent/Edit");

		const match = useMatch(Patterns.editEvent);

		const data = await this.eventService.get(match.params.uid);

		this.setState({ loading: false, configCodes: templates.rows, dataView, data });
	};

	formatPageTitle(): string {
		const data = this.state.data;

		let result = "";

		if (data.configCode) {
			const item = this.state.configCodes
				.find(x => x.configCode == data.configCode);

			if (item) {
				result += item.name + " ";
			}
		}

		if (data.id) {
			result += "№ " + data.id;
		}

		return result;
	}

	createRefForKey(key: string): React.RefObject<unknown> {
		const ref: React.RefObject<unknown> = React.createRef();
		this._refsByKey.set(key, ref);
		return ref;
	}

	handleSave(): void {
		this._refsByKey.forEach((ref) => {
			(ref.current as PaneComponent).save();
		});

		this.fetchData();
	}

	handlePublish = async (): Promise<void> => {
		const t = (key: string) => i18next.getFixedT(null, Locale.Namespace)(key);

		const result = await this.operation.confirm(async () => {
			const { uid } = this.getRouteProps();

			return await this.eventService.publish(uid);
		}, t("publish.confirm.content"));

		if (result.success) {
			this.fetchData();
		}
	};

	handleCancel = async (): Promise<void> => {

		const t = (key: string) => i18next.getFixedT(null, Locale.Namespace)(key);

		const result = await this.operation.confirm(async () => {
			const { uid } = this.getRouteProps();

			return await this.eventService.cancel(uid);
		}, t("cancel.confirm.content"));

		if (result.success) {
			this.fetchData();
		}
	};

	handleTabChange = (tabKey: string): void => {
		const { uid } = this.getRouteProps();

		const path = RouteBuilder.editEvent(uid, tabKey);

		this.props.navigate(path);
	};

	render = (): React.ReactNode => {
		const t = (key: string) => i18next.getFixedT(null, Locale.Namespace)(key),
			{ tabKey } = this.getRouteProps(),
			{ loading, data, dataView } = this.state;

		if (data.id == null) return null;

		// todo: load from Configuration
		let toolbar: React.ReactNode;

		toolbar = (<>
			<Button type="primary" onClick={() => this.handlePublish()}>{t("button.publish")}</Button>
			{/* <Button icon={Icon.Check} onClick={() => this.handleSave()}>{t("button.save")}</Button> */}
			<Button onClick={() => this.handleCancel()}>{t("button.cancel")}</Button>
		</>);

		/* if (data.statusCode == "draft") {
			toolbar = (<>
				<Button type="primary" onClick={() => this.handlePublish()}>Опубликовать</Button>&#xA0;
				<Button icon="check" onClick={() => this.handleSave()}>Сохранить</Button>
			</>);
		}
		if (data.statusCode == "published") {
			toolbar = (<>
				<Button onClick={() => this.handleCancel()}>Отменить</Button> &#xA0;
			</>);
		} */

		const paneProps: DataPaneProps<IEvent> = {
			entityTypeCode: EntityTypeCode.event,
			entityUid: data.uid,
			data,
			/* , ref: this.createRefForKey(pane.key) */
		};

		return (
			<Page title={<>
				<Toolbar float="right">
					{toolbar}
				</Toolbar>

				<DataBreadcrumb items={[]} />
				<PageHeader>{this.formatPageTitle()} <Tag color="green">{data.statusCode}</Tag></PageHeader>
			</>}>
				<Spin spinning={loading}>

					<h3 title={data.name} className="single-line-text">{data.name}</h3>

					<DataTabs
						tabKey={tabKey}
						panes={dataView?.panes}
						onTabChange={this.handleTabChange}
						disabled={(pane, index) => index > 0 && !data?.uid}
						paneProps={paneProps}
					/>

				</Spin>
			</Page>
		);
	};
}

export default withNavigate(withParams(PageEditEvent));
