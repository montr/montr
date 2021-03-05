import * as React from "react";
import i18next from "i18next";
import { Button, Modal, message, Tag, Spin } from "antd";
import { ApiResult, DataView } from "@montr-core/models";
import { EventService, EventTemplateService } from "../services";
import { Page, PaneComponent, Toolbar, PageHeader, DataBreadcrumb, DataTabs } from "@montr-core/components";
import { MetadataService } from "@montr-core/services";
import { IEvent } from "../models";
import { RouteBuilder } from "../module";
import { RouteComponentProps } from "react-router";

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

class _EditEvent extends React.Component<Props, State> {

	private _metadataService = new MetadataService();
	private _eventTemplateService = new EventTemplateService();
	private _eventService = new EventService();

	private _refsByKey: Map<string, any> = new Map<string, React.RefObject<any>>();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
			configCodes: [],
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._eventTemplateService.abort();
		await this._eventService.abort();
	};

	fetchData = async () => {
		const templates = await this._eventTemplateService.list();

		// todo: get metadata key from server
		const dataView = await this._metadataService.load("PrivateEvent/Edit");

		const data = await this._eventService.get(this.props.match.params.uid);

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

	handleSave() {
		this._refsByKey.forEach((ref) => {
			(ref.current as PaneComponent).save();
		});

		this.fetchData();
	}

	handlePublish() {
		const t = (key: string) => i18next.getFixedT(null, "tendr")(key);

		Modal.confirm({
			title: t("confirm.title"),
			content: t("publish.confirm.content"),
			onOk: () => {
				this._eventService
					.publish(this.props.match.params.uid)
					.then((result: ApiResult) => {
						message.success(t("operation.success"));
						this.fetchData();
					})
					.catch(error => {
						message.error(t("operation.error"));
					});
			}
		});
	}

	handleCancel() {
		const t = (key: string) => i18next.getFixedT(null, "tendr")(key);

		Modal.confirm({
			title: t("confirm.title"),
			content: t("cancel.confirm.content"),
			onOk: () => {
				this._eventService
					.cancel(this.props.match.params.uid)
					.then((result: ApiResult) => {
						message.success(t("operation.success"));
						this.fetchData();
					});
			}
		});
	}

	handleTabChange = (tabKey: string) => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.editEvent(uid, tabKey);

		this.props.history.replace(path);
	};

	render = () => {
		const t = (key: string) => i18next.getFixedT(null, "tendr")(key),
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
						tabProps={{ data /* , ref: this.createRefForKey(pane.key) */ }}
					/>

				</Spin>
			</Page>
		);
	};
}

const EditEvent = _EditEvent;

export default EditEvent;
