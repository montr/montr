import * as React from "react";
import Cookies from "universal-cookie";
import { ICompany } from "../models";
import { CompanyAPI } from "../services";
import { CompanyContextProps, CompanyContext } from "./";
import { Guid } from "@montr-core/.";

interface State {
	currentCompany?: ICompany;
	companyList: ICompany[];
}

const cookie_name = "current_company_uid";

export class CompanyContextProvider extends React.Component<any, State> {

	private _cookies = new Cookies();

	constructor(props: any) {
		super(props);

		this.state = {
			companyList: [],
		};
	}

	componentDidMount = async () => {
		const companyList: ICompany[] = await CompanyAPI.list();

		let currentCompanyUid: Guid,
			currentCompany: ICompany;

		if (companyList && Array.isArray(companyList) && companyList.length > 0) {

			var storageValue = this._cookies.get(cookie_name);

			if (Guid.isValid(storageValue)) {
				currentCompanyUid = new Guid(storageValue);
			}

			currentCompany = companyList.find(x => x.uid == currentCompanyUid);

			if (!currentCompany) {
				currentCompany = companyList[0];
			}

			if (currentCompany && currentCompany.uid != currentCompanyUid) {
				this.switchCompany(currentCompany);
			}
		}

		this.setState({ companyList, currentCompany });
	}

	switchCompany = (currentCompany: ICompany): void => {

		this._cookies.set(cookie_name, currentCompany.uid.toString(), { domain: ".montr.io" });

		this.setState({ currentCompany });
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
