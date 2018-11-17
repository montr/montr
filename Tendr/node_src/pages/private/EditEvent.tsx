import * as React from "react";

import { Tabs, Button, Icon } from "antd";

import { IEvent, EventAPI, EventTemplateAPI, IEventTemplate, IPane } from "../../api";
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

export class EditEvent extends React.Component<IEditEventProps, IEditEventState> {

	constructor(props: IEditEventProps) {
		super(props);

		this.state = { data: {}, configCodes: [] };
	}

	componentWillMount() {
		const id = this.props.params.id;

		EventAPI.get(id)
			.then(data => this.setState({ data }));

		EventTemplateAPI.load()
			.then((data) => this.setState({ configCodes: data }));
	}

	buildPageTitle(): string {
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

	render() {
		const data = this.state.data;

		// todo: load from Metadata
		const panes: IPane<IEvent>[] = [
			{ key: "tab_1", title: "Информация", icon: "profile", component: EditEventPane },
			{ key: "tab_team", title: "Команда", icon: "team", component: null },
			{ key: "tab_5", title: "Позиции", icon: "table", component: null },
			{ key: "tab_4", title: "Тендерная комиссия (команда?)", component: null },
			{ key: "tab_6", title: "Критерии оценки (анкета?)", component: null },
			{ key: "tab_7", title: "История изменений", icon: "eye", component: null },
			{ key: "tab_2", title: "Документы (поле?)", component: null },
			{ key: "tab_3", title: "Контактные лица (поле?)", component: null },
		]

		function callback(key: string) {
			console.log(key);
		}

		const toolbar = (
			<div>
				<Button type="primary">Опубликовать</Button>&#xA0;
				<Button icon="check">Сохранить</Button>
			</div>
		);

		return (
			<Page title={this.buildPageTitle()} toolbar={toolbar}>
				<h2 title={data.name} style={{ overflow: "hidden", whiteSpace: "nowrap", textOverflow: "ellipsis" }}>{data.name}</h2>

				<Tabs size="small" onChange={callback}>
					{panes.map(pane => {
						return (
							<Tabs.TabPane key={pane.key}
								tab={<span>{pane.icon && <Icon type={pane.icon} />} {pane.title}</span>}>
								{pane.component && React.createElement(pane.component, { data: data })}
							</Tabs.TabPane>
						);
					})}
				</Tabs>

			</Page>
		);
	}
}
