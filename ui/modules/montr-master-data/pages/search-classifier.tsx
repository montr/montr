import * as React from "react";
import { Page, DataTable, PageHeader } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Constants } from "@montr-core/.";
import { Icon, Button, Breadcrumb, Menu, Dropdown } from "antd";
import { Link } from "react-router-dom";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService } from "../services";
import { NotificationService } from "@montr-core/services";

interface IRouteProps {
	configCode: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	selectedRowKeys: string[] | number[];
	postParams: any;
}

class _SearchClassifier extends React.Component<IProps, IState> {

	private _classifierService = new ClassifierService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			selectedRowKeys: [],
			postParams: {}
		};
	}

	componentDidMount = async () => {
		this.setPostParams();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.match.params.configCode !== prevProps.match.params.configCode ||
			this.props.currentCompany !== prevProps.currentCompany) {
			this.setPostParams();
		}
	}

	private setPostParams = async () => {
		const { currentCompany } = this.props,
			{ configCode } = this.props.match.params;

		this.setState({
			postParams: {
				configCode, companyUid: currentCompany ? currentCompany.uid : null
			}
		});
	}

	private delete = async () => {
		const rowsAffected = await this._classifierService
			.delete(this.props.currentCompany.uid, this.state.selectedRowKeys);

		this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

		this.setPostParams(); // to force table refresh
	}

	private export = async () => {
		// todo: show export dialog: all pages, current page, export format
		await this._classifierService.export(this.props.currentCompany.uid, {
			configCode: this.props.match.params.configCode
		});
	}

	private onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	}

	render() {
		const { currentCompany } = this.props,
			{ configCode } = this.props.match.params;

		if (!currentCompany) return null;

		const menu = (
			<Menu>
				<Menu.Item key="0">
					<Link to={`/classifiers/${configCode}`}>{configCode}</Link>
				</Menu.Item>
			</Menu>
		);

		return (
			<Page
				title={
					<>
						<Breadcrumb>
							<Breadcrumb.Item><Icon type="home" /></Breadcrumb.Item>
							<Breadcrumb.Item><Link to={`/classifiers`}>Справочники</Link></Breadcrumb.Item>
							<Breadcrumb.Item>
								<Dropdown overlay={menu} trigger={['click']}>
									<a className="ant-dropdown-link" href="#">
										{configCode} <Icon type="down" />
									</a>
								</Dropdown>
							</Breadcrumb.Item>
						</Breadcrumb>

						<PageHeader>{configCode}</PageHeader>
					</>
				}
				toolbar={
					<>
						<Link to={`/classifiers/${configCode}/new`}>
							<Button type="primary"><Icon type="plus" /> Добавить</Button>
						</Link>
						&#xA0;<Button onClick={this.delete}><Icon type="delete" /> Удалить</Button>
						&#xA0;<Button onClick={this.export}><Icon type="export" /> Экспорт</Button>
					</>
				}>

				<DataTable
					viewId={`ClassifierList/Grid/${configCode}`}
					loadUrl={`${Constants.baseURL}/classifier/list/`}
					postParams={this.state.postParams}
					rowKey="uid"
					onSelectionChange={this.onSelectionChange}
				/>

			</Page>
		);
	}
}

export const SearchClassifier = withCompanyContext(_SearchClassifier);
