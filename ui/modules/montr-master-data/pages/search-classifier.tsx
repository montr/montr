import * as React from "react";
import { Page, DataTable, PageHeader } from "@montr-core/components";
import { NotificationService } from "@montr-core/services";
import { RouteComponentProps } from "react-router";
import { Constants } from "@montr-core/.";
import { Icon, Button, Breadcrumb, Menu, Dropdown, Tree, Row, Col } from "antd";
const { TreeNode } = Tree;
import { AntTreeNodeSelectedEvent } from "antd/lib/tree";
import { Link } from "react-router-dom";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService } from "../services";
import { IClassifierType } from "../models";

interface IRouteProps {
	configCode: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	type: IClassifierType;
	selectedRowKeys: string[] | number[];
	postParams: any;
}

class _SearchClassifier extends React.Component<IProps, IState> {

	private _classifierService = new ClassifierService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			type: {
				hierarchyType: "None"
			},
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

	componentWillUnmount = async () => {
		await this._classifierService.abort();
	}

	private setPostParams = async () => {
		const { currentCompany } = this.props,
			{ configCode } = this.props.match.params;

		const type = await this.fetchClassifierType(configCode);

		this.setState({
			type: type,
			postParams: {
				typeCode: configCode, companyUid: currentCompany ? currentCompany.uid : null
			}
		});
	}

	private fetchClassifierType = async (typeCode: string): Promise<IClassifierType> => {
		const data = await this._classifierService.types();

		return data.rows.find(x => x.code == typeCode);
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

	private onTreeSelect = async (selectedKeys: string[], e: AntTreeNodeSelectedEvent) => {
		console.log(selectedKeys, e);
	}

	render() {
		const { currentCompany } = this.props,
			{ type, postParams } = this.state;

		if (!currentCompany || !type) return null;

		const menu = (
			<Menu>
				<Menu.Item key="0">
					<Link to={`/classifiers/${type.code}`}>{type.name}</Link>
				</Menu.Item>
			</Menu>
		);

		// todo: настройки
		// 1. как выглядит дерево - списком или деревом
		// 2. прятать дерево
		// 3. показывать или нет группы в таблице
		// 4. показывать планарную таблицу без групп
		let tree;
		if (type.hierarchyType == "Groups") {
			tree = (
				<Tree.DirectoryTree
					multiple={false}
					defaultExpandAll
					onSelect={this.onTreeSelect}
				>
					<TreeNode title="parent 0" key="0-0" >
						<TreeNode title="leaf 0-0" key="0-0-0" />
						<TreeNode title="leaf 0-1" key="0-0-1" />
					</TreeNode>
					<TreeNode title="parent 1" key="0-1">
						<TreeNode title="leaf 1-0" key="0-1-0" />
						<TreeNode title="leaf 1-1" key="0-1-1" />
					</TreeNode>
				</Tree.DirectoryTree>
			);
		}

		const table = (
			<DataTable
				viewId={`ClassifierList/Grid/${type.code}`}
				loadUrl={`${Constants.baseURL}/classifier/list/`}
				postParams={postParams}
				rowKey="uid"
				onSelectionChange={this.onSelectionChange}
			/>
		);

		const content = (tree)
			? (
				<Row>
					<Col span={4}>
						{tree}
					</Col>
					<Col span={20}>
						{table}
					</Col>
				</Row>
			)
			: (table);

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
										{type.name} <Icon type="down" />
									</a>
								</Dropdown>
							</Breadcrumb.Item>
						</Breadcrumb>

						<PageHeader>{type.name}</PageHeader>
					</>
				}
				toolbar={
					<>
						<Link to={`/classifiers/${type.code}/new`}>
							<Button type="primary"><Icon type="plus" /> Добавить</Button>
						</Link>
						&#xA0;<Button onClick={this.delete}><Icon type="delete" /> Удалить</Button>
						&#xA0;<Button onClick={this.export}><Icon type="export" /> Экспорт</Button>
					</>
				}>

				{content}

			</Page>
		);
	}
}

export const SearchClassifier = withCompanyContext(_SearchClassifier);
