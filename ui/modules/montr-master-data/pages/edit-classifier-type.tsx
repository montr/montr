import * as React from "react";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { Page, DataForm, PageHeader } from "@montr-core/components";
import { Guid, IFormField, ISelectField, IOption } from "@montr-core/models";
import { RouteComponentProps } from "react-router";
import { ClassifierService } from "../services";
import { IClassifierType } from "../models";
import { ClassifierBreadcrumb } from "../components";
import { Spin } from "antd";


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

	private _classifierService = new ClassifierService();

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

	private fetchData = async () => {
		const { typeCode, currentCompany, uid } = this.props;

		if (currentCompany) {

			const types = await this._classifierService.types(currentCompany.uid);
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
			{ key: "name", type: "textarea", name: "Наименование", required: true },
			{
				key: "hierarchyType", type: "select", name: "Иерархия", required: true,
				options: [
					{ value: "None", name: "None" },
					{ value: "Items", name: "Items" },
					{ value: "Groups", name: "Groups" },
				]
			},
		];

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} types={types} />
				<PageHeader>{type.name}</PageHeader>
			</>}>
				<Spin spinning={this.state.loading}>
					{fields && <DataForm fields={fields} data={type} onSave={this.save} />}
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
