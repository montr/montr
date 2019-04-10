import * as React from "react";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { Page, DataForm, PageHeader } from "@montr-core/components";
import { Guid } from "@montr-core/models";
import { RouteComponentProps } from "react-router";
import { ClassifierTypeService } from "../services";
import { IClassifierType } from "../models";
import { ClassifierBreadcrumb } from "../components";
import { Spin, Tabs } from "antd";


interface IProps extends CompanyContextProps {
	typeCode: string;
	uid?: Guid;
}

interface IState {
	loading: boolean;
	type?: IClassifierType;
	types?: IClassifierType[];
}

class _EditClassifierTypeForm extends React.Component<IProps, IState> {

	private _classifierTypeService = new ClassifierTypeService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			type: {
				hierarchyType: "None"
			}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.typeCode !== prevProps.typeCode ||
			this.props.currentCompany !== prevProps.currentCompany) {

			await this.fetchData();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
	}

	private fetchData = async () => {
		const { typeCode, currentCompany, uid } = this.props;

		if (currentCompany) {

			const types = await this._classifierTypeService.list(currentCompany.uid);
			const type = types.rows.find(x => x.code == typeCode);

			this.setState({ loading: false, type, types: types.rows });
		}
	}

	private save = async (values: IClassifierType) => {
	}

	render = () => {
		const { typeCode } = this.props,
			{ type, types } = this.state;

		const fields = [
			{ key: "code", type: "string", name: "Код", required: true },
			{ key: "name", type: "textarea", name: "Наименование", rows: 2, required: true },
			{ key: "description", type: "textarea", name: "Описание" },
			{
				key: "hierarchyType", type: "select", name: "Иерархия", required: true,
				options: [
					{ value: "None", name: "Нет" },
					{ value: "Groups", name: "Группы" },
					{ value: "Items", name: "Элементы" },
				]
			},
		];

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} types={types} />
				<PageHeader>{type.name}</PageHeader>
			</>}>
				<Spin spinning={this.state.loading}>
					<Tabs>
						<Tabs.TabPane key="1" tab="Информация">
							{fields && <DataForm fields={fields} data={type} onSave={this.save} />}
						</Tabs.TabPane>
						<Tabs.TabPane key="2" tab="Иерархия"></Tabs.TabPane>
						<Tabs.TabPane key="3" tab="Поля"></Tabs.TabPane>
						<Tabs.TabPane key="4" tab="История изменений"></Tabs.TabPane>
					</Tabs>
				</Spin>
			</Page>
		)
	}
}

const EditClassifierTypeForm = withCompanyContext(_EditClassifierTypeForm);

interface IRouteProps {
	typeCode: string;
	uid?: string;
}

export class EditClassifierType extends React.Component<RouteComponentProps<IRouteProps>> {
	render = () => {

		const { typeCode, uid } = this.props.match.params;

		return <EditClassifierTypeForm typeCode={typeCode} uid={uid ? new Guid(uid) : null} />
	}
}
