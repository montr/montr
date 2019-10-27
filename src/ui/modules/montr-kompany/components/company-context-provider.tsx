import * as React from "react";
import Cookies from "universal-cookie";
import { ICompany } from "../models";
import { CompanyService, } from "../services";
import { CompanyContextProps, CompanyContext } from "./";
import { Constants } from "@montr-core/.";
import { Guid } from "@montr-core/models";
import { NavigationService, NotificationService, AuthService } from "@montr-core/services";

interface State {
	currentCompany?: ICompany;
	companyList: ICompany[];
}

export class CompanyContextProvider extends React.Component<any, State> {

	private _cookies = new Cookies();
	private _navigation = new NavigationService();
	private _notification = new NotificationService();
	private _authService = new AuthService();
	private _companyService = new CompanyService();

	constructor(props: any) {
		super(props);

		this.state = {
			companyList: [],
		};
	}

	componentDidMount = async () => {
		const user = await this._authService.getUser();

		if (user) {
			await this.switchCompany();
		}
	}

	componentWillUnmount = async () => {
		await this._companyService.abort();
	}

	registerCompany = (): void => {
		const returnUrl = encodeURI(this._navigation.getUrl());
		this._navigation.navigate(
			`${Constants.publicURL}/register/company/?${Constants.returnUrlParam}=${returnUrl}`);
	}

	manageCompany = (): void => {
		this._navigation.navigate(`${Constants.publicURL}/manage/`);
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
