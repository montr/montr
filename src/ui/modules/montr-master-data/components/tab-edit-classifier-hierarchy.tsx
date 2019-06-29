import * as React from "react";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { IClassifierType, IClassifierGroup, IClassifier } from "../models";
import { DataTableUpdateToken } from "@montr-core/components";
import { Alert } from "antd";

interface IProps extends CompanyContextProps {
	type: IClassifierType;
	data: IClassifier;
	onDataChange?: (values: IClassifier) => void
}

interface IState {
	loading: boolean;
	groupEditData?: IClassifierGroup;
	updateTableToken: DataTableUpdateToken;
}

class _TabEditClassifierHierarchy extends React.Component<IProps, IState> {

	render() {
		const { type } = this.props;

		return (<>
			<Alert type={type.hierarchyType == "Groups" ? "info" : "warning"}
				message="Настройка иерархий групп доступна для типов справочников, у которых на вкладке «Информация» выбран тип иерархии «Группы»." />

		</>);
	}


}

export const TabEditClassifierHierarchy = withCompanyContext(_TabEditClassifierHierarchy);
