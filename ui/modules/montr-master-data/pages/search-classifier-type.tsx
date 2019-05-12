import * as React from "react";
import { Page, DataTable, PageHeader, Toolbar } from "@montr-core/components";
import { Constants } from "@montr-core/.";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierBreadcrumb } from "../components";
import { Link } from "react-router-dom";
import { Button, Icon } from "antd";
import { ClassifierTypeService } from "../services";
import { NotificationService } from "@montr-core/services";
import { IClassifierType } from "../models";
import { IDataResult } from "@montr-core/models";

interface IProps extends CompanyContextProps {
}

interface IState {
	selectedRowKeys: string[] | number[];
	postParams: any;
}

class _SearchClassifierType extends React.Component<IProps, IState> {

	private _classifierTypeService = new ClassifierTypeService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			selectedRowKeys: [],
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

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
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

	private onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	}

	private delete = async () => {
		try {
			const rowsAffected = await this._classifierTypeService
				.delete(this.props.currentCompany.uid, this.state.selectedRowKeys);

			this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

			this.setPostParams(); // to force table refresh
		} catch (error) {
			this._notificationService.error("Ошибка при удалении данных", error.message);
		}
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		return await this._classifierTypeService.post(loadUrl, Object.assign(postParams, this.state.postParams));
	}

	render = () => {
		const { postParams } = this.state;

		if (postParams.companyUid == null) return null;

		return (
			<Page
				title={<>
					<Toolbar float="right">
						<Link to={`/classifiers/add`}>
							<Button type="primary"><Icon type="plus" /> Добавить</Button>
						</Link>
						<Button onClick={this.delete}><Icon type="delete" /> Удалить</Button>
					</Toolbar>

					<ClassifierBreadcrumb />
					<PageHeader>Справочники</PageHeader>
				</>}>

				<DataTable
					rowKey="uid"
					viewId={`ClassifierType/Grid/`}
					loadUrl={`${Constants.baseURL}/classifierType/list/`}
					// postParams={this.state.postParams}
					onLoadData={this.onLoadTableData}
					onSelectionChange={this.onSelectionChange}
				/>

			</Page>
		);
	}
}

export const SearchClassifierType = withCompanyContext(_SearchClassifierType);
