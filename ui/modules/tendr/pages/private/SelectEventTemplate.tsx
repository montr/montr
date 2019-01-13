import * as React from "react";
import { Button, List } from "antd";
import { IEventTemplate, EventTemplateAPI, EventAPI } from "../../api";
import { Redirect } from "react-router-dom";
import { Page } from "@montr-core/components";

interface Props {
}

interface State {
	newId?: number;
	data: IEventTemplate[];
}

export class SelectEventTemplate extends React.Component<Props, State> {
	constructor(props: Props) {
		super(props);
		this.state = { data: [] };
	}

	componentDidMount() {
		EventTemplateAPI.load()
			.then((data) => {
				this.setState({ data });
			});
	}

	handleSelect(configCode: string) {
		EventAPI
			.create(configCode)
			.then((newId: number) => this.setState({ newId: newId }));
	}

	render() {
		if (this.state.newId) {
			return <Redirect to={`/events/edit/${this.state.newId}`} />
		}

		return (
			<Page title="Выберите шаблон процедуры">

				<div style={{ width: "50%" }}>

					<List size="small" bordered dataSource={this.state.data} itemLayout="horizontal"
						renderItem={(item: IEventTemplate) => (
							<List.Item
								actions={[
									<Button onClick={() => this.handleSelect(item.configCode)}>Выбрать</Button>
								]}>
								<List.Item.Meta
									title={item.name}
									description={item.description}
								/>
							</List.Item>
						)}
					/>
				</div>

			</Page>
		);
	}
}
