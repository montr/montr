import * as React from "react";
import { withTranslation, WithTranslation } from 'react-i18next';
import { Button, Modal, message, Tag } from "antd";
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

interface Props extends WithTranslation, RouteComponentProps<RouteProps> {
}

interface State {
	data: IEvent;
	dataView: DataView<IEvent>;
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
			data: {},
			dataView: { id: "" },
			configCodes: []
		};
	}

	componentDidMount() {
		this.fetchConfigCodes();
		this.fetchMetadata();
		this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._eventTemplateService.abort();
		await this._eventService.abort();
	};

	fetchConfigCodes = async () => {
		const templates = await this._eventTemplateService.list();

		this.setState({ configCodes: templates.rows });
	};

	fetchData = async () => {
		this.setState({ data: await this._eventService.get(this.props.match.params.uid) });
	};

	fetchMetadata = async () => {
		// todo: get metadata key from server
		const dataView = await this._metadataService.load("PrivateEvent/Edit");

		this.setState({ dataView: dataView });
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
		const { t } = this.props;

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
		const { t } = this.props;

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
		const { t } = this.props;
		const { tabKey } = this.props.match.params;
		const { data, dataView } = this.state;

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

				<h3 title={data.name} className="single-line-text">{data.name}</h3>

				<DataTabs
					tabKey={tabKey}
					panes={dataView.panes}
					onTabChange={this.handleTabChange}
					data={data}
				/>

			</Page>
		);
	};
}

const EditEvent = withTranslation("tendr")(_EditEvent);

export default EditEvent;
