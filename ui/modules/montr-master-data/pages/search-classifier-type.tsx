import * as React from "react";
import { Page, DataTable, PageHeader } from "@montr-core/components";
import { Constants } from "@montr-core/.";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierBreadcrumb } from "../components";

interface IProps extends CompanyContextProps {
}

interface IState {
	postParams: any;
}

class _SearchClassifierType extends React.Component<IProps, IState> {

	constructor(props: IProps) {
		super(props);

		this.state = {
			postParams: {
				companyUid: null
			}
		};
	}

	componentDidMount = async () => {
		await this.setPostParams();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.currentCompany !== prevProps.currentCompany) {
			await this.setPostParams();
		}
	}

	private setPostParams = async () => {
		const { currentCompany } = this.props,
			{ postParams } = this.state;

		const companyUid = currentCompany ? currentCompany.uid : null;

		this.setState({
			postParams: {
				companyUid: companyUid
			}
		});
	}

	render = () => {
		const { postParams } = this.state;

		if (postParams.companyUid == null) return null;

		return (
			<Page title={<>
				<ClassifierBreadcrumb />
				<PageHeader>Справочники</PageHeader>
			</>}>

				<DataTable
					viewId={`ClassifierType/Grid/`}
					loadUrl={`${Constants.baseURL}/classifierType/list/`}
					postParams={this.state.postParams}
					rowKey="code"
				/>

			</Page>
		);
	}
}

export const SearchClassifierType = withCompanyContext(_SearchClassifierType);
