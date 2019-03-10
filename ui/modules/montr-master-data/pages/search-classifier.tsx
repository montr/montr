import * as React from "react";
import { Page, DataTable, PageHeader } from "@montr-core/components";
import { NotificationService } from "@montr-core/services";
import { RouteComponentProps } from "react-router";
import { Constants } from "@montr-core/.";
import { Icon, Button, Breadcrumb, Menu, Dropdown, Tree, Row, Col, Select } from "antd";
const { TreeNode } = Tree;
import { Link } from "react-router-dom";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService } from "../services";
import { IClassifierType, IClassifierTree, IClassifierGroup } from "../models";

interface IRouteProps {
	typeCode: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	type: IClassifierType;
	trees: IClassifierTree[];
	groups: IClassifierGroup[];
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
			trees: [],
			groups: [],
			selectedRowKeys: [],
			postParams: {}
		};
	}

	componentDidMount = async () => {
		this.setPostParams();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.match.params.typeCode !== prevProps.match.params.typeCode ||
			this.props.currentCompany !== prevProps.currentCompany) {
			this.setPostParams();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierService.abort();
	}

	private setPostParams = async () => {
		const { currentCompany } = this.props,
			{ typeCode } = this.props.match.params;

		const type = await this.fetchClassifierType(typeCode);

		let trees: IClassifierTree[] = [],
			groups: IClassifierGroup[] = [];

		if (type && type.hierarchyType == "Groups") {
			trees = await this.fetchClassifierTrees(typeCode);

			if (trees && trees.length > 0) {
				groups = await this.fetchClassifierGroups(typeCode, trees[0].code)
			}
		}

		this.setState({
			type: type,
			trees: trees,
			groups: groups,
			postParams: {
				typeCode: typeCode, companyUid: currentCompany ? currentCompany.uid : null
			}
		});
	}

	private fetchClassifierType = async (typeCode: string): Promise<IClassifierType> => {
		const { currentCompany } = this.props;

		if (!currentCompany) return null;

		const data = await this._classifierService.types(currentCompany.uid);

		return data.rows.find(x => x.code == typeCode);
	}

	private fetchClassifierTrees = async (typeCode: string): Promise<IClassifierTree[]> => {
		const { currentCompany } = this.props;

		if (!currentCompany) return null;

		const data = await this._classifierService.trees(currentCompany.uid, typeCode);

		return data.rows;
	}

	private fetchClassifierGroups = async (typeCode: string, treeCode: string): Promise<IClassifierGroup[]> => {
		const { currentCompany } = this.props

		if (!currentCompany) return null;

		return await this._classifierService.groups(currentCompany.uid, typeCode, treeCode);
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
			configCode: this.props.match.params.typeCode
		});
	}

	private onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	}

	private onTreeSelect = async (selectedKeys: string[]) => {
		console.log(selectedKeys);
	}

	private buildGroupsTree = (groups: IClassifierGroup[]) => {
		return groups && groups.map(x => {
			return (
				<TreeNode title={`${x.code} - ${x.name}`} key={x.code} isLeaf={!x.children}>
					{this.buildGroupsTree(x.children)}
				</TreeNode>
			);
		});
	}

	render() {
		const { currentCompany } = this.props,
			{ type, trees, groups, postParams } = this.state;

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
		if (type.hierarchyType == "Groups" && trees && trees.length > 0) {
			tree = <div style={{ overflowX: "auto", height: "90vh" }}>
				{<Select defaultValue="default" style={{ width: "95%" }}>
					{trees.map(x => <Select.Option key={x.code}>{x.name || x.code}</Select.Option>)}
				</Select>}
				<Tree.DirectoryTree
					multiple={false}
					defaultExpandAll
					onSelect={this.onTreeSelect}
					showIcon={false}
				>
					<TreeNode title={trees[0].name || trees[0].code}>
						{this.buildGroupsTree(groups)}
					</TreeNode>
				</Tree.DirectoryTree>
			</div>;
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
					<Col span={6}>
						{tree}
					</Col>
					<Col span={18}>
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
