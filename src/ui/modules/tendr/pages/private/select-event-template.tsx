import * as React from "react";
import { Button, List } from "antd";
import { EventTemplateService, EventService } from "../../services";
import { Redirect } from "react-router-dom";
import { Page } from "@montr-core/components";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { IEventTemplate } from "modules/tendr/models";
import { IApiResult, Guid } from "@montr-core/models";
import { RouteBuilder } from ".";

interface State {
	newUid?: Guid;
	data: IEventTemplate[];
}

class _SelectEventTemplate extends React.Component<CompanyContextProps, State> {

	private _eventTemplateService = new EventTemplateService();
	private _eventService = new EventService();

	constructor(props: CompanyContextProps) {
		super(props);

		this.state = {
			data: []
		};
	}

	componentDidMount = async () => {
		this.setState({ data: await this._eventTemplateService.list() });
	}

	componentWillUnmount = async () => {
		await this._eventTemplateService.abort();
		await this._eventService.abort();
	}

	private _handleSelect = async (configCode: string) => {
		const result: IApiResult = await this._eventService.insert({
			configCode: configCode,
			companyUid: this.props.currentCompany.uid
		});

		this.setState({ newUid: result.uid });
	}

	render = () => {
		const { newUid } = this.state;

		if (newUid) {
			return <Redirect to={RouteBuilder.editClassifier(newUid.toString())} />
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
