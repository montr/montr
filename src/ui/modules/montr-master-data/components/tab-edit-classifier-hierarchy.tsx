import * as React from "react";
import { Alert, Button, Icon } from "antd";
import { Constants } from "@montr-core/.";
import { DataTableUpdateToken, Toolbar, DataTable } from "@montr-core/components";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { IClassifierType, IClassifierGroup, IClassifier } from "../models";
import { IDataResult } from "@montr-core/models";
import { ClassifierLinkService } from "@montr-master-data/services";

interface IProps extends CompanyContextProps {
	type: IClassifierType;
	data: IClassifier;
	onDataChange?: (values: IClassifier) => void
}

interface IState {
	groupEditData?: IClassifierGroup;
	updateTableToken: DataTableUpdateToken;
}

class _TabEditClassifierHierarchy extends React.Component<IProps, IState> {

	_classifierLinkService = new ClassifierLinkService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.currentCompany !== prevProps.currentCompany ||
			this.props.type !== prevProps.type ||
			this.props.data !== prevProps.data) {
			await this.refreshTable();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierLinkService.abort();
	}

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { currentCompany, type, data } = this.props;

		if (currentCompany && type.code) {

			const params = {
				companyUid: currentCompany.uid,
				typeCode: type.code,
				itemUid: data.uid,
				...postParams
			};

			return await this._classifierLinkService.post(loadUrl, params);
		}

		return null;
	}

	add = () => {
	}

	render() {
		const { type } = this.props,
			{ updateTableToken } = this.state;

		if (!type) return null;

		return (<>
			<Alert type={type.hierarchyType == "Groups" ? "info" : "warning"}
				message="Настройка иерархий групп доступна для типов справочников, у которых на вкладке «Информация» выбран тип иерархии «Группы»." />

			{type.hierarchyType == "Groups" && (<>
				<Toolbar>
					<Button onClick={this.add}><Icon type="plus" /> Добавить</Button>
				</Toolbar>

				<div style={{ clear: "both" }} />

				<DataTable
					rowKey="code"
					viewId="ClassifierLink/Grid"
					loadUrl={`${Constants.baseURL}/classifierLink/list/`}
					onLoadData={this.onLoadTableData}
					updateToken={updateTableToken}
				/>
			</>)}
		</>);
	}
}

export const TabEditClassifierHierarchy = withCompanyContext(_TabEditClassifierHierarchy);
