import * as React from "react";
import { ICompany } from "../api";

export interface CompanyContextProps {
	currentCompany?: ICompany,
	companyList: ICompany[],
	switchCompany: (company: ICompany) => void
}

const defaultState: CompanyContextProps = {
	companyList: [],
	switchCompany: (company: ICompany) => { }
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
