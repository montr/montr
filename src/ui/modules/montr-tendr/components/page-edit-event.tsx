import { DataBreadcrumb, DataTabs, Page, PageHeader, PaneComponent, Toolbar } from "@montr-core/components";
import { DataPaneProps, DataView } from "@montr-core/models";
import { MetadataService, OperationService } from "@montr-core/services";
import { Button, Spin, Tag } from "antd";
import i18next from "i18next";
import * as React from "react";
import { RouteComponentProps, useMatch, useNavigate } from "react-router";
import { IEvent } from "../models";
import { EntityTypeCode, Locale, Patterns, RouteBuilder } from "../module";
import { EventService, EventTemplateService } from "../services";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
	data: IEvent;
	dataView?: DataView<IEvent>;
	configCodes: IEvent[];
}

export default class PageEditEvent extends React.Component<Props, State> {

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

	createRefForKey(key: string): React.RefObject<Props> {
		const ref: React.RefObject<Props> = React.createRef();
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
			return await this.eventService.publish(this.props.match.params.uid);
		}, t("publish.confirm.content"));

		if (result.success) {
			this.fetchData();
		}
	};

	handleCancel = async (): Promise<void> => {

		const t = (key: string) => i18next.getFixedT(null, Locale.Namespace)(key);

		const result = await this.operation.confirm(async () => {
			return await this.eventService.cancel(this.props.match.params.uid);
		}, t("cancel.confirm.content"));

		if (result.success) {
			this.fetchData();
		}
	};

	handleTabChange = (tabKey: string): void => {
		const { uid } = this.props.match.params;

		const navigate = useNavigate();

		const path = RouteBuilder.editEvent(uid, tabKey);

		navigate(path);
	};

	render = (): React.ReactNode => {
		const t = (key: string) => i18next.getFixedT(null, Locale.Namespace)(key),
			{ tabKey } = this.props.match.params,
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
