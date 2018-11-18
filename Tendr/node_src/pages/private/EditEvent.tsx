import * as React from "react";

import { Tabs, Button, Icon, Modal, message } from "antd";

import { IEvent, EventAPI, EventTemplateAPI, IEventTemplate, IPane, IPaneProps, IApiResult } from "../../api";
import { Page } from "../../components/";
import { EditEventPane } from ".";

interface IEditEventProps {
	params: {
		id: number
	};
}

interface IEditEventState {
	data: IEvent;
	configCodes: IEventTemplate[];
}

export interface IPaneComponent {
	save(): void;
}

export class EditEvent extends React.Component<IEditEventProps, IEditEventState> {

	private _refsByKey: Map<string, any> = new Map<string, React.RefObject<any>>();

	constructor(props: IEditEventProps) {
		super(props);

		this.state = { data: {}, configCodes: [] };
	}

	componentWillMount() {
		this.fetchData();

		EventTemplateAPI.load()
			.then((data) => this.setState({ configCodes: data }));
	}

	private fetchData() {
		EventAPI.get(this.props.params.id)
			.then(data => this.setState({ data }));
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
			let pane: IPaneComponent = ref.current;
			pane.save();
		});
	}

	handlePublish() {
		Modal.confirm({
			title: "Подтверждение операции",
			content: "Вы действительно хотите опубликовать событие?",
			iconType: "question",
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
			iconType: "question",
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
		
		if (data.id == null) return null;

		// todo: load from Metadata
		const panes: IPane<IEvent>[] = [
			{ key: "tab_1", title: "Информация", icon: "profile", component: EditEventPane },
			{ key: "tab_team", title: "Команда", icon: "team", component: null },
			{ key: "tab_5", title: "Позиции", icon: "table", component: null },
			{ key: "tab_7", title: "История изменений", icon: "eye", component: null },
			{ key: "tab_4", title: "Тендерная комиссия (команда?)", component: null },
			{ key: "tab_6", title: "Критерии оценки (анкета?)", component: null },
			{ key: "tab_2", title: "Документы (поле?)", component: null },
			{ key: "tab_3", title: "Контактные лица (поле?)", component: null },
		]

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
					{panes.map(pane => {

						let component: React.ReactElement<IPaneProps<IEvent>>;
						if (pane.component) {
							component = React.createElement(pane.component,
								{ data: data, ref: this.createRefForKey(pane.key) });
						}

						return (
							<Tabs.TabPane key={pane.key}
								tab={<span>{pane.icon && <Icon type={pane.icon} />} {pane.title}</span>}>
								{component}
							</Tabs.TabPane>
						);
					})}
				</Tabs>

			</Page>
		);
	}
}
