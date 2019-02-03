import * as React from "react";
import { Button, List } from "antd";
import { EventTemplateService, EventService } from "../../services";
import { Redirect } from "react-router-dom";
import { Page } from "@montr-core/components";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { IEventTemplate } from "modules/tendr/models";

interface State {
	newId?: number;
	data: IEventTemplate[];
}

class _SelectEventTemplate extends React.Component<CompanyContextProps, State> {

	private _eventTemplateService = new EventTemplateService();
	private _eventService = new EventService();

	constructor(props: CompanyContextProps) {
		super(props);
		this.state = { data: [] };
	}

	componentDidMount = async () => {
		this.setState({ data: await this._eventTemplateService.list() });
	}

	componentWillUnmount = async () => {
		await this._eventTemplateService.abort();
		await this._eventService.abort();
	}

	private _handleSelect = async (configCode: string) => {
		const newId: number = await this._eventService.create({
			configCode: configCode,
			companyUid: this.props.currentCompany.uid
		});

		this.setState({ newId: newId });
	}

	render = () => {
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
									<Button onClick={() => this._handleSelect(item.configCode)}>Выбрать</Button>
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

export const SelectEventTemplate = withCompanyContext(_SelectEventTemplate);
