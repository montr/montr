import * as React from "react";
import { Tabs, Button, Icon, Modal, message } from "antd";
import { IApiResult } from "@montr-core/api/";
import { IEvent, EventAPI, EventTemplateAPI, IEventTemplate, IPaneProps, MetadataAPI, IDataView } from "../../api";
import { IPaneComponent } from "../../panes/";
import { Page } from "@montr-core/components";

interface IEditEventProps {
	params: {
		id: number
	};
}

interface IEditEventState {
	data: IEvent;
	dataView: IDataView<IEvent>;
	configCodes: IEventTemplate[];
}

export class EditEvent extends React.Component<IEditEventProps, IEditEventState> {

	private _refsByKey: Map<string, any> = new Map<string, React.RefObject<any>>();

	constructor(props: IEditEventProps) {
		super(props);

		this.state = { data: {}, dataView: { id: "" }, configCodes: [] };
	}

	componentWillMount() {
		this.fetchConfigCodes();
		this.fetchData();
		this.fetchMetadata();
	}

	private fetchConfigCodes() {
		EventTemplateAPI.load()
			.then((data) => this.setState({ configCodes: data }));
	}

	private fetchData() {
		EventAPI.get(this.props.params.id)
			.then(data => this.setState({ data }));
	}

	private fetchMetadata() {
		MetadataAPI
			.load("PrivateEvent/Edit")
			.then((dataView) => this.setState({ dataView }));
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

	createRefForKey(key: string): React.RefObject<IEditEventProps> {
		const ref: React.RefObject<IEditEventProps> = React.createRef()
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
				EventAPI
					.publish(this.props.params.id)
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
				EventAPI
					.cancel(this.props.params.id)
					.then((result: IApiResult) => {
						message.success("Событие отменено: " + JSON.stringify(result));
						this.fetchData();
					});
			}
		});
	}

	render() {
		const data = this.state.data;
		const dataView = this.state.dataView;

		if (data.id == null) return null;

		// todo: load from Configuration
		let toolbar: any;

		if (data.statusCode == "draft") {
			toolbar = (
				<div>
					<Button type="primary" onClick={() => this.handlePublish()}>Опубликовать</Button>&#xA0;
					<Button icon="check" onClick={() => this.handleSave()}>Сохранить</Button>
				</div>
			);
		}
		if (data.statusCode == "published") {
			toolbar = (
				<div>
					<Button onClick={() => this.handleCancel()}>Отменить</Button>&#xA0;
				</div>
			);
		}

		return (
			<Page title={this.formatPageTitle()} toolbar={toolbar}>

				<h2 title={data.name} className="single-line-text">{data.name}</h2>

				<Tabs size="small">
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
