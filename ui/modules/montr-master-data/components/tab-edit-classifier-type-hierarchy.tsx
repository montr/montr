import * as React from "react";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { DataTable } from "@montr-core/components";
import { IClassifierType } from "../models";
import { Constants } from "@montr-core/.";

interface IProps extends CompanyContextProps {
	type: IClassifierType;
}

interface IState {
	loading: boolean;
}

class _TabEditClassifierTypeHierarchy extends React.Component<IProps, IState> {
	render() {
		return (
			<DataTable
				viewId="ClassifierType/Grid/Hierarchy"
				loadUrl={`${Constants.baseURL}/classifierType/hierarchy/`}
			/>
		);
	}
}

export const TabEditClassifierTypeHierarchy = withCompanyContext(_TabEditClassifierTypeHierarchy);
