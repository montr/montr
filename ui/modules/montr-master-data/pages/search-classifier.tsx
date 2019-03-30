import * as React from "react";
import { Page, DataTable, PageHeader } from "@montr-core/components";
import { NotificationService } from "@montr-core/services";
import { RouteComponentProps } from "react-router";
import { Constants } from "@montr-core/.";
import { Icon, Button, Breadcrumb, Menu, Dropdown, Tree, Select, Radio, Layout } from "antd";
import { Link } from "react-router-dom";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService } from "../services";
import { IClassifierType, IClassifierTree, IClassifierGroup } from "../models";
import { RadioChangeEvent } from "antd/lib/radio";
import { AntTreeNode } from "antd/lib/tree";

interface IRouteProps {
	typeCode: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	types: IClassifierType[];
	type: IClassifierType;
	trees?: IClassifierTree[];
	treeCode?: string,
	groups?: IClassifierGroup[];
	selectedRowKeys: string[] | number[];
	postParams: any;
}

class _SearchClassifier extends React.Component<IProps, IState> {

	private _classifierService = new ClassifierService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			types: [],
			type: {
				hierarchyType: "None"
			},
			selectedRowKeys: [],
			postParams: {
				depth: "0"
			}
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

		if (!currentCompany) return;

		const types = await this.fetchClassifierTypes();
		const type = types.find(x => x.code == typeCode);

		let trees: IClassifierTree[] = [],
			treeCode: string,
			groups: IClassifierGroup[] = [];

		if (type) {
			if (type.hierarchyType == "Groups") {
				trees = await this.fetchClassifierTrees(typeCode);

				if (trees && trees.length > 0) {
					treeCode = trees[0].code;
					groups = await this.fetchClassifierGroups(typeCode, treeCode)
				}
			}

			if (type.hierarchyType == "Items") {
				groups = await this.fetchClassifierGroups(typeCode, null)
			}

			this.setState({
				types: types,
				type: type,
				trees: trees,
				treeCode: treeCode,
				groups: groups,
				postParams: {
					companyUid: currentCompany ? currentCompany.uid : null,
					typeCode: typeCode,
					treeCode: treeCode
				}
			});
		}
	}

	private fetchClassifierTypes = async (): Promise<IClassifierType[]> => {
		const { currentCompany } = this.props;

		const data = await this._classifierService.types(currentCompany.uid);

		return data.rows;
	}

	private fetchClassifierTrees = async (typeCode: string): Promise<IClassifierTree[]> => {
		const { currentCompany } = this.props;

		const data = await this._classifierService.trees(currentCompany.uid, typeCode);

		return data.rows;
	}

	private fetchClassifierGroups = async (typeCode: string, treeCode: string, parentCode?: string): Promise<IClassifierGroup[]> => {
		const { currentCompany } = this.props

		return await this._classifierService.groups(currentCompany.uid, typeCode, treeCode, parentCode);
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
			typeCode: this.props.match.params.typeCode
		});
	}

	private onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	}

	private onTreeLoadData = async (node: AntTreeNode) => new Promise(async (resolve) => {
		const group: IClassifierGroup = node.props.dataRef;

		const { type, treeCode } = this.state;

		const children = await this.fetchClassifierGroups(type.code, treeCode, group.code)

		group.children = children;

		this.setState({
			groups: [...this.state.groups],
		});

		resolve();
	})

	private onTreeSelect = (selectedKeys: string[]) => {
		const { postParams } = this.state;
		this.setState({ postParams: { ...postParams, groupCode: selectedKeys[0] } });
	}

	private onDepthChange = (e: RadioChangeEvent) => {
		const { postParams } = this.state;
		this.setState({ postParams: { ...postParams, depth: e.target.value } });
	}

	private buildGroupsTree = (groups: IClassifierGroup[]) => {
		return groups && groups.map(x => {
			return (
				<Tree.TreeNode title={`${x.code} - ${x.name}`} key={x.code} dataRef={x}>
					{x.children && this.buildGroupsTree(x.children)}
				</Tree.TreeNode>
			);
		});
	}

	render() {
		const { currentCompany } = this.props,
			{ types, type, trees, groups, postParams } = this.state;

		if (!currentCompany || !type || !postParams.typeCode) return null;

		// todo: настройки:
		// 1. как выглядит дерево - списком или деревом (?)
		// 2. прятать дерево
		// 3. показывать или нет группы в таблице
		// 4. показывать планарную таблицу без групп
		let treeSelect;
		if (trees && trees.length > 0) {
			treeSelect = (
				<Select defaultValue="default" size="small">
					{trees.map(x => <Select.Option key={x.code}>{x.name || x.code}</Select.Option>)}
				</Select>
			);
		}

		let tree;
		if (type.hierarchyType != "None" && groups.length > 0) {
			tree = (
				<Tree blockNode
					loadData={this.onTreeLoadData}
					onSelect={this.onTreeSelect}>
					{this.buildGroupsTree(groups)}
				</ Tree>
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
			? (<>
				<Layout.Sider width="360" theme="light" collapsible={false} style={{ overflowX: "auto", height: "100vh" }}>
					{tree}
				</Layout.Sider>
				<Layout.Content className="bg-white" style={{ paddingTop: 0, paddingRight: 0 }}>
					{table}
				</Layout.Content>
			</>)
			: (table);


		const typeSelectorMenu = (
			<Menu>
				{types.map(x => <Menu.Item key={x.code}>
					<Link to={`/classifiers/${x.code}/`}>{x.name}</Link>
				</Menu.Item>)}
			</Menu>
		);

		const typeSelector = (
			<Dropdown overlay={typeSelectorMenu} trigger={['click']}>
				<a className="ant-dropdown-link" href="#">
					{type.name} <Icon type="down" />
				</a>
			</Dropdown>
		);

		return (
			<Page
				title={<>
					<Breadcrumb>
						<Breadcrumb.Item><Icon type="home" /></Breadcrumb.Item>
						<Breadcrumb.Item><Link to={`/classifiers/`}>Справочники</Link></Breadcrumb.Item>
						<Breadcrumb.Item>
							{typeSelector}
						</Breadcrumb.Item>
					</Breadcrumb>

					<PageHeader>{type.name}</PageHeader>
				</>}
				toolbar={<>
					<Link to={`/classifiers/${type.code}/new`}>
						<Button type="primary"><Icon type="plus" /> Добавить</Button>
					</Link>
					&#xA0;<Button onClick={this.delete}><Icon type="delete" /> Удалить</Button>
					&#xA0;<Button onClick={this.export}><Icon type="export" /> Экспорт</Button>
				</>}>

				<Layout>
					<Layout.Header className="bg-white" style={{ padding: "0" }}>
						{treeSelect}
						&#xA0;
						<div style={{ float: "right" }}>
							<Radio.Group defaultValue="0" size="small" onChange={this.onDepthChange}>
								<Radio.Button value="0"><Icon type="folder" /> Группа</Radio.Button>
								<Radio.Button value="1"><Icon type="cluster" /> Иерархия</Radio.Button>
							</Radio.Group>
						</div>
					</Layout.Header>
					<Layout hasSider={!!tree} className="bg-white">
						{content}
					</Layout>
				</Layout>
			</Page>
		);
	}
}

export const SearchClassifier = withCompanyContext(_SearchClassifier);
