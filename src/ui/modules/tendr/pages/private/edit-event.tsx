import * as React from "react";
import { Tabs, Button, Icon, Modal, message } from "antd";
import { IApiResult, IDataView, IPaneProps } from "@montr-core/models";
import { EventService, EventTemplateService } from "../../services";
import { Page, IPaneComponent, Toolbar, PageHeader, DataBreadcrumb } from "@montr-core/components";
import { MetadataService } from "@montr-core/services";
import { IEvent, IEventTemplate } from "modules/tendr/models";

import * as panes from "../../components"
import { RouteBuilder } from ".";
import { CompanyContextProps } from "@kompany/components";
import { RouteComponentProps } from "react-router";

const componentToClass: Map<string, React.ComponentClass> = new Map<string, React.ComponentClass>();

// todo: register tabs outside
// todo: fix cast
componentToClass.set("panes/private/EditEventPane", panes.TabEditEvent);
componentToClass.set("panes/private/InvitationPane", panes.TabEditInvitations as unknown as React.ComponentClass);

interface IRouteProps {
	id?: string;
	tabKey?: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	data: IEvent;
	dataView: IDataView<IEvent>;
	configCodes: IEventTemplate[];
}

export class EditEvent extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _eventTemplateService = new EventTemplateService();
	private _eventService = new EventService();

	private _refsByKey: Map<string, any> = new Map<string, React.RefObject<any>>();

	constructor(props: IProps) {
		super(props);

		this.state = { data: {}, dataView: { id: "" }, configCodes: [] };
	}

	componentWillMount() {
		this.fetchConfigCodes();
		this.fetchMetadata();
		this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._eventTemplateService.abort();
		await this._eventService.abort();
	}

	private fetchConfigCodes = async () => {
		this.setState({ configCodes: await this._eventTemplateService.list() });
	}

	private fetchData = async () => {
		this.setState({ data: await this._eventService.get(this.props.match.params.id) });
	}

	private resolveComponent = (component: string): React.ComponentClass => {
		return componentToClass.get(component);
	}

	private fetchMetadata = async () => {
		// todo: get metadata key from server
		const dataView = await this._metadataService.load("PrivateEvent/Edit", this.resolveComponent);

		this.setState({ dataView: dataView });
	}

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

	createRefForKey(key: string): React.RefObject<IProps> {
		const ref: React.RefObject<IProps> = React.createRef()
		this._refsByKey.set(key, ref);
		return ref;
	}

	handleSave() {
		this._refsByKey.forEach((ref) => {
			(ref.current as IPaneComponent).save();
		});
		// this.fetchData();
	}

	handlePublish() {
		Modal.confirm({
			title: "Подтверждение операции",
			content: "Вы действительно хотите опубликовать событие?",
			onOk: () => {
				this._eventService
					.publish(this.props.match.params.id)
					.then((result: IApiResult) => {
						message.success("Событие опубликовано: " + JSON.stringify(result));
						this.fetchData();
					});
			}
		});
	}

	handleCancel() {
		Modal.confirm({
			title: "Подтверждение операции",
			content: "Вы действительно хотите отменить событие?",
			onOk: () => {
				this._eventService
					.cancel(this.props.match.params.id)
					.then((result: IApiResult) => {
						message.success("Событие отменено: " + JSON.stringify(result));
						this.fetchData();
					});
			}
		});
	}

	handleTabChange = (tabKey: string) => {
		const { id } = this.props.match.params;

		const path = RouteBuilder.editClassifier(id, tabKey);

		this.props.history.replace(path)
	}

	render = () => {
		const { tabKey } = this.props.match.params;
		const { data, dataView } = this.state;

		if (data.id == null) return null;

		// todo: load from Configuration
		let toolbar: React.ReactNode;

		if (data.statusCode == "draft") {
			toolbar = (<>
				<Button type="primary" onClick={() => this.handlePublish()}>Опубликовать</Button>&#xA0;
				<Button icon="check" onClick={() => this.handleSave()}>Сохранить</Button>
			</>);
		}
		if (data.statusCode == "published") {
			toolbar = (<>
				<Button onClick={() => this.handleCancel()}>Отменить</Button> &#xA0;
			</>);
		}

		return (
			<Page title={<>
				<Toolbar float="right">
					{toolbar}
				</Toolbar>

				<DataBreadcrumb items={[]} />
				<PageHeader>{this.formatPageTitle()}</PageHeader>
			</>}>

				<h3 title={data.name} className="single-line-text">{data.name}</h3>

				<Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
					{dataView && dataView.panes && dataView.panes.map(pane => {

						let component: React.ReactElement<IPaneProps<IEvent>>;
						if (pane.component) {
							component = React.createElement(pane.component,
								{ data: data, ref: this.createRefForKey(pane.key) });
						}

						return (
							<Tabs.TabPane key={pane.key}
								tab={<span>{pane.icon && <Icon type={pane.icon} />} {pane.name}</span>}>
								{component}
							</Tabs.TabPane>
						);
					})}
				</Tabs>

			</Page>
		);
	}
}
