import * as React from "react";

import { Tabs, Button, Icon } from "antd";

import { IEvent, EventAPI, EventTemplateAPI, IEventTemplate, IPane, IPaneProps } from "../../api";
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
		EventAPI.get(this.props.params.id)
			.then(data => this.setState({ data }));

		EventTemplateAPI.load()
			.then((data) => this.setState({ configCodes: data }));
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
	}

	render() {
		const data = this.state.data;

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

		function callback(key: string) {
			console.log(key);
		}

		const toolbar = (
			<div>
				<Button type="primary" onClick={() => this.handlePublish()}>Опубликовать</Button>&#xA0;
				<Button icon="check" onClick={() => this.handleSave()}>Сохранить</Button>
			</div>
		);

		if (data.id == null) return null;

		return (
			<Page title={this.formatPageTitle()} toolbar={toolbar}>

				<h2 title={data.name} className="single-line-text">{data.name}</h2>

				<Tabs size="small" onChange={callback}>
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
