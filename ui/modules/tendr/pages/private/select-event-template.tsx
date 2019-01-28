import * as React from "react";
import { Button, List } from "antd";
import { IEventTemplate, EventTemplateAPI, EventAPI } from "../../api";
import { Redirect } from "react-router-dom";
import { Page } from "@montr-core/components";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";

interface State {
	newId?: number;
	data: IEventTemplate[];
}

class _SelectEventTemplate extends React.Component<CompanyContextProps, State> {
	constructor(props: CompanyContextProps) {
		super(props);
		this.state = { data: [] };
	}

	componentDidMount() {
		EventTemplateAPI.load()
			.then((data) => {
				this.setState({ data });
			});
	}

	handleSelect = async (configCode: string) => {
		const newId: number = await EventAPI.create({
			configCode: configCode,
			companyUid: this.props.currentCompany.uid
		});

		this.setState({ newId: newId });
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

export const SelectEventTemplate = withCompanyContext(_SelectEventTemplate);
