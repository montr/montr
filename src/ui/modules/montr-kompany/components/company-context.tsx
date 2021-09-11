import { Guid } from "@montr-core/models";
import * as React from "react";
import { Company } from "../models";

export interface CompanyContextProps {
	currentCompany?: Company;
	companyList: Company[];
	registerCompany: () => void;
	manageCompany: () => void;
	switchCompany: (companyUid: Guid) => void;
}

const defaultState: CompanyContextProps = {
	companyList: [],
	registerCompany: () => { },
	manageCompany: () => { },
	switchCompany: (companyUid: Guid) => { }
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
