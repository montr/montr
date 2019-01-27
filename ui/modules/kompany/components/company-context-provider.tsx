import * as React from "react";
import Cookies from "universal-cookie";
import { ICompany } from "../models";
import { CompanyAPI } from "../services";
import { CompanyContextProps, CompanyContext } from "./";
import { Guid } from "@montr-core/models";
import { NavigationService } from "@montr-core/services";

interface State {
	currentCompany?: ICompany;
	companyList: ICompany[];
}

const cookie_name = "current_company_uid";

export class CompanyContextProvider extends React.Component<any, State> {

	private _cookies = new Cookies();
	private _navigation = new NavigationService();

	constructor(props: any) {
		super(props);

		this.state = {
			companyList: [],
		};
	}

	componentDidMount = async () => {
		await this.loadCompanyList();
	}

	loadCompanyList = async () => {
		const companyList: ICompany[] = await CompanyAPI.list();

		this.setState({ companyList });

		this.switchCompany();
	}

	registerCompany = (): void => {
		const currentUrl = this._navigation.getUrl();

		const redirectUrl = "http://kompany.montr.io:5010/register/?return_url=" + encodeURI(currentUrl);

		this._navigation.navigate(redirectUrl);
	}

	switchCompany = (companyUid?: Guid): void => {
		const { companyList } = this.state;

		let currentCompanyUid: Guid,
			currentCompany: ICompany;

		if (companyList && Array.isArray(companyList) && companyList.length > 0) {

			if (companyUid) {
				currentCompanyUid = companyUid;
			}
			else {
				var storageValue = this._cookies.get(cookie_name);

				if (Guid.isValid(storageValue)) {
					currentCompanyUid = new Guid(storageValue);
				}
			}

			currentCompany = companyList.find(x => x.uid == currentCompanyUid);

			if (!currentCompany) {
				currentCompany = companyList[0];
			}

			if (currentCompany /* && currentCompany.uid != currentCompanyUid */) {
				this._cookies.set(cookie_name, currentCompany.uid.toString(), { domain: ".montr.io" });

				this.setState({ currentCompany });
			}
		}
	}

	render = () => {
		const { currentCompany, companyList } = this.state;

		const context: CompanyContextProps = {
			currentCompany: currentCompany,
			companyList: companyList,
			loadCompanyList: this.loadCompanyList,
			registerCompany: this.registerCompany,
			switchCompany: this.switchCompany
		};

		return (
			<CompanyContext.Provider value={context}>
				{this.props.children}
			</CompanyContext.Provider>
		);
	}
}
