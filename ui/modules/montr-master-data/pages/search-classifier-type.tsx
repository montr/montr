import * as React from "react";
import { Page, DataTable, PageHeader } from "@montr-core/components";
import { NotificationService } from "@montr-core/services";
import { RouteComponentProps } from "react-router";
import { Constants } from "@montr-core/.";
import { Icon, Button, Breadcrumb, Menu, Dropdown, Tree, Row, Col, Select, Radio, Layout } from "antd";
import { Link } from "react-router-dom";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService } from "../services";
import { IClassifierType, IClassifierTree, IClassifierGroup } from "../models";
import { RadioChangeEvent } from "antd/lib/radio";

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
				<Breadcrumb>
					<Breadcrumb.Item><Icon type="home" /></Breadcrumb.Item>
					<Breadcrumb.Item>Справочники</Breadcrumb.Item>
				</Breadcrumb>

				<PageHeader>Справочники</PageHeader>
			</>}>

				<DataTable
					viewId={`ClassifierType/Grid/`}
					loadUrl={`${Constants.baseURL}/classifier/types/`}
					postParams={this.state.postParams}
					rowKey="code"
				/>

			</Page>
		);
	}
}

export const SearchClassifierType = withCompanyContext(_SearchClassifierType);
