import { Constants } from "@montr-core/.";
import { Guid } from "@montr-core/models";
import { AuthService, NavigationService, NotificationService } from "@montr-core/services";
import { User } from "oidc-client";
import * as React from "react";
import Cookies from "universal-cookie";
import { Company } from "../models";
import { UserCompanyService } from "../services";
import { CompanyContext, CompanyContextProps } from "./";

interface Props {
	children: React.ReactNode;
}

interface State {
	currentCompany?: Company;
	companyList: Company[];
}

export class CompanyContextProvider extends React.Component<Props, State> {

	private readonly cookies = new Cookies();
	private readonly navigation = new NavigationService();
	private readonly notification = new NotificationService();
	private readonly authService = new AuthService();
	private readonly userCompanyService = new UserCompanyService();

	constructor(props: Props) {
		super(props);

		this.state = {
			companyList: [],
		};
	}

	componentDidMount = async (): Promise<void> => {
		this.authService.userManager.events.addUserLoaded(this.onUserLoaded);
		this.authService.userManager.events.addUserUnloaded(this.onUserUnloaded);

		await this.switchCompany();
	};

	componentWillUnmount = async (): Promise<void> => {
		this.authService.userManager.events.removeUserLoaded(this.onUserLoaded);
		this.authService.userManager.events.removeUserUnloaded(this.onUserUnloaded);

		await this.userCompanyService.abort();
	};

	onUserLoaded = async (user: User): Promise<void> => {
		await this.switchCompany();
	};

	onUserUnloaded = async (): Promise<void> => {
		await this.switchCompany();
	};

	registerCompany = (): void => {
		const returnUrl = encodeURI(this.navigation.getUrl());
		this.navigation.navigate(
			`${Constants.publicURL}/register/company/?${Constants.returnUrlParam}=${returnUrl}`);
	};

	manageCompany = (): void => {
		this.navigation.navigate(`${Constants.publicURL}/manage/`);
	};

	switchCompany = async (companyUid?: Guid): Promise<void> => {

		const companyList = await this.getCompanyList();

		if (companyList && Array.isArray(companyList) && companyList.length > 0) {

			const cookieCompanyUid = this.getCookieCompanyUid();

			const setCookieCompanyUid = companyUid || cookieCompanyUid;

			const currentCompany =
				companyList.find(x => x.uid == setCookieCompanyUid) || companyList[0];

			if (currentCompany && currentCompany.uid != cookieCompanyUid) {
				this.setCookieCompanyUid(currentCompany.uid);
			}

			this.setState({ currentCompany, companyList });
		}
	};

	getCompanyList = async (): Promise<Company[]> => {
		try {
			const user = await this.authService.getUser();

			const result = user ? await this.userCompanyService.list() : [];

			return result;

		} catch (error) {
			console.log(error);
			this.notification.error(`Error loading companies list`, error.message);
		}
	};

	getCookieCompanyUid = (): Guid => {
		const storageValue = this.cookies.get(Constants.cookieName);

		if (Guid.isValid(storageValue)) {
			return new Guid(storageValue);
		}

		return null;
	};

	setCookieCompanyUid = (companyUid: Guid): void => {
		this.cookies.set(Constants.cookieName, companyUid.toString(), {
			domain: Constants.cookieDomain, path: "/"
		});
	};

	render = (): React.ReactNode => {
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
	};
}
