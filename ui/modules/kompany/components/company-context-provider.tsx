import * as React from "react";
import { ICompany, CompanyAPI } from "../api";
import { CompanyContextProps, CompanyContext } from "./";

interface State {
	currentCompany?: ICompany;
	companyList: ICompany[];
}

export class CompanyContextProvider extends React.Component<any, State> {
	constructor(props: any) {
		super(props);

		this.state = {
			companyList: [],
		};
	}

	componentDidMount = async () => {
		const data: ICompany[] = await CompanyAPI.list();

		this.setState({ companyList: data });
	}

	switchCompany = (company: ICompany): void => {
		console.log("switch to ->", company);
	}

	render = () => {
		const { currentCompany, companyList } = this.state;

		const context: CompanyContextProps = {
			currentCompany: currentCompany,
			companyList: companyList,
			switchCompany: this.switchCompany
		};

		return (
			<CompanyContext.Provider value={context}>
				{this.props.children}
			</CompanyContext.Provider>
		);
	}
}
