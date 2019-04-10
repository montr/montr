import * as React from "react";
import { RouteComponentProps, Redirect } from "react-router";
import { Spin, Tabs } from "antd";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { Page, DataForm, PageHeader } from "@montr-core/components";
import { Guid } from "@montr-core/models";
import { ClassifierTypeService } from "../services";
import { IClassifierType } from "../models";
import { ClassifierBreadcrumb } from "../components";

interface IProps extends CompanyContextProps {
	uid?: Guid;
}

interface IState {
	loading: boolean;
	types?: IClassifierType[];
	data: IClassifierType;
	redirect?: string;
}

class _EditClassifierTypeForm extends React.Component<IProps, IState> {

	private _classifierTypeService = new ClassifierTypeService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {
				// todo: load defaults with metadata
				name: "Новый справочник",
				hierarchyType: "None"
			}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.currentCompany !== prevProps.currentCompany) {
			await this.fetchData();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
	}

	private fetchData = async () => {
		const { currentCompany, uid } = this.props;

		if (currentCompany) {

			const types = await this._classifierTypeService.list(currentCompany.uid);

			const data = (uid) ? types.rows.find(x => x.uid == uid) : this.state.data;

			this.setState({ loading: false, data, types: types.rows });
		}
	}

	private save = async (values: IClassifierType) => {

		const { uid } = this.props;
		const { uid: companyUid } = this.props.currentCompany;

		if (uid) {
			const data = { uid, ...values };
			const rowsUpdated = await this._classifierTypeService.update(companyUid, data);

			if (rowsUpdated != 1) throw new Error();
			this.setState({ data });
		}
		else {
			const uid: Guid = await this._classifierTypeService.insert(companyUid, values);

			this.setState({ redirect: `/classifiers/edit/${uid}` });
		}
	}

	render = () => {
		if (this.state.redirect) {
			return <Redirect to={this.state.redirect} />
		}

		const { uid } = this.props,
			{ loading, data, types } = this.state;

		// todo: fetch from metadata api
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

		let title;
		if (loading) {
			title = <>
				<ClassifierBreadcrumb types={types} />
				<PageHeader>&#xA0;</PageHeader>
			</>;
		}
		else {
			title = <>
				{(uid)
					? <ClassifierBreadcrumb types={types} type={data} item={data} />
					: <ClassifierBreadcrumb item={{ name: "Добавление справочника" }} />
				}
				<PageHeader>{data.name}</PageHeader>
			</>;
		}

		const otherTabsDisabled = !data.uid;

		return (
			<Page title={title}>
				<Spin spinning={this.state.loading}>
					<Tabs>
						<Tabs.TabPane key="1" tab="Информация">
							{fields && <DataForm fields={fields} data={data} onSave={this.save} />}
						</Tabs.TabPane>
						<Tabs.TabPane key="2" tab="Иерархия" disabled={otherTabsDisabled}></Tabs.TabPane>
						<Tabs.TabPane key="3" tab="Поля" disabled={otherTabsDisabled}></Tabs.TabPane>
						<Tabs.TabPane key="4" tab="История изменений" disabled={otherTabsDisabled}></Tabs.TabPane>
					</Tabs>
				</Spin>
			</Page>
		)
	}
}

const EditClassifierTypeForm = withCompanyContext(_EditClassifierTypeForm);

interface IRouteProps {
	uid?: string;
}

export class EditClassifierType extends React.Component<RouteComponentProps<IRouteProps>> {
	render = () => {

		const { uid } = this.props.match.params;

		return <EditClassifierTypeForm uid={uid ? new Guid(uid) : null} />
	}
}
