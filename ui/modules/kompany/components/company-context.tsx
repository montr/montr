import * as React from "react";
import { ICompany } from "../models";
import { Guid } from "@montr-core/models";

export interface CompanyContextProps {
	currentCompany?: ICompany,
	companyList: ICompany[],
	loadCompanyList: () => void,
	registerCompany: () => void,
	switchCompany: (companyUid: Guid) => void,
}

const defaultState: CompanyContextProps = {
	companyList: [],
	loadCompanyList: () => { },
	registerCompany: () => { },
	switchCompany: (companyUid: Guid) => { },
};

export const CompanyContext = React.createContext<CompanyContextProps>(defaultState);

export function withCompanyContext<P extends CompanyContextProps>(Component: React.ComponentType<P>) {
	return (props: Pick<P, Exclude<keyof P, keyof CompanyContextProps>>) => (
		<CompanyContext.Consumer>
			{(ctx) => (
				<Component {...props} {...ctx as P} />
			)}
		</CompanyContext.Consumer>
	);
}
