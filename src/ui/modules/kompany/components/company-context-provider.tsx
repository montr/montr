import * as React from "react";
import Cookies from "universal-cookie";
import { ICompany } from "../models";
import { CompanyService, Constants } from "../services";
import { CompanyContextProps, CompanyContext } from "./";
import { Guid } from "@montr-core/models";
import { NavigationService, NotificationService } from "@montr-core/services";

interface State {
	currentCompany?: ICompany;
	companyList: ICompany[];
}

export class CompanyContextProvider extends React.Component<any, State> {

	private _cookies = new Cookies();
	private _navigation = new NavigationService();
	private _notification = new NotificationService();
	private _companyService = new CompanyService();

	constructor(props: any) {
		super(props);

		this.state = {
			companyList: [],
		};
	}

	componentDidMount = async () => {
		this.switchCompany();
	}

	componentWillUnmount = async () => {
		await this._companyService.abort();
	}

	registerCompany = (): void => {
		const returnUrl = encodeURI(this._navigation.getUrl());
		this._navigation.navigate(
			`${Constants.baseURL}/register/company/?${Constants.returnUrlParam}=${returnUrl}`);
	}

	manageCompany = (): void => {
		this._navigation.navigate(`${Constants.baseURL}/manage/`);
	}

	switchCompany = async (companyUid?: Guid) => {

		await this._loadCompanyList();

		const { companyList } = this.state;

		if (companyList && Array.isArray(companyList) && companyList.length > 0) {

			const cookieCompanyUid = this._getCookieCompanyUid();

			const setCookieCompanyUid = companyUid || cookieCompanyUid;

			const company =
				companyList.find(x => x.uid == setCookieCompanyUid) || companyList[0];

			if (company && company.uid != cookieCompanyUid) {
				this._setCookieCompanyUid(company.uid);
			}

			this.setState({ currentCompany: company });
		}
	}

	private _loadCompanyList = async () => {
		try {
			this.setState({ companyList: await this._companyService.list() });
		} catch (error) {
			this._notification.error(`Ошибка при загрузке списка организаций`, error.message);
		}
	}

	private _getCookieCompanyUid = (): Guid => {
		var storageValue = this._cookies.get(Constants.cookieName);

		if (Guid.isValid(storageValue)) {
			return new Guid(storageValue);
		}

		return null;
	}

	private _setCookieCompanyUid = (companyUid: Guid): void => {
		this._cookies.set(Constants.cookieName, companyUid.toString(), {
			domain: Constants.cookieDomain, path: "/"
		});
	}

	render = () => {
		const { currentCompany, companyList } = this.state;

		const context: CompanyContextProps = {
			currentCompany: currentCompany,
			companyList: companyList,
			registerCompany: this.registerCompany,
			manageCompany: this.manageCompany,
			switchCompany: this.switchCompany
		};

		return (
			<CompanyContext.Provider value={context}>
				{this.props.children}
			</CompanyContext.Provider>
		);
	}
}
