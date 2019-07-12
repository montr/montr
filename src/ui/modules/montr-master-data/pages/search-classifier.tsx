import * as React from "react";
import { Page, PageHeader, Toolbar, DataTable, DataTableUpdateToken } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { Icon, Button, Tree, Select, Radio, Layout, Modal, Spin } from "antd";
import { Constants } from "@montr-core/.";
import { Guid, IDataResult } from "@montr-core/models";
import { NotificationService } from "@montr-core/services";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService, ClassifierTypeService, ClassifierGroupService, ClassifierTreeService } from "../services";
import { IClassifierType, IClassifierGroup, IClassifierTree } from "../models";
import { RadioChangeEvent } from "antd/lib/radio";
import { AntTreeNode, AntTreeNodeSelectedEvent, AntTreeNodeExpandedEvent } from "antd/lib/tree";
import { ClassifierBreadcrumb, ModalEditClassifierGroup } from "../components";

interface IRouteProps {
	typeCode: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	types: IClassifierType[];
	type?: IClassifierType;
	trees?: IClassifierGroup[];
	treeUid?: Guid,
	groups?: IClassifierGroup[];
	selectedGroup?: IClassifierGroup;
	groupEditData?: IClassifierGroup;
	expandedKeys: string[];
	selectedRowKeys: string[] | number[];
	depth: string; // todo: make enum
	updateTableToken: DataTableUpdateToken;
}

class _SearchClassifier extends React.Component<IProps, IState> {
	_classifierTypeService = new ClassifierTypeService();
	_classifierTreeService = new ClassifierTreeService();
	_classifierGroupService = new ClassifierGroupService();
	_classifierService = new ClassifierService();
	_notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			types: [],
			expandedKeys: [],
			selectedRowKeys: [],
			depth: "0",
			updateTableToken: { date: new Date() }
		};
	}

	componentDidMount = async () => {
		await this.loadClassifierTypes();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.currentCompany !== prevProps.currentCompany) {
			// todo: check if selected type belongs to company (show 404)
			await this.loadClassifierTypes();
		}
		else if (this.props.match.params.typeCode !== prevProps.match.params.typeCode) {

			this.setState({
				selectedRowKeys: [],
				selectedGroup: null
			});

			await this.loadClassifierType();
			await this.refreshTable(true);
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
		await this._classifierTreeService.abort();
		await this._classifierGroupService.abort();
		await this._classifierService.abort();
	}

	loadClassifierTypes = async () => {
		const { currentCompany } = this.props;

		if (currentCompany) {
			const types = (await this._classifierTypeService.list(currentCompany.uid)).rows;

			this.setState({ types });

			await this.loadClassifierType();
		}
	}

	loadClassifierType = async () => {
		const { currentCompany } = this.props,
			{ typeCode } = this.props.match.params;

		if (currentCompany) {
			const type = await this._classifierTypeService.get(currentCompany.uid, { typeCode });

			this.setState({ type });

			await this.loadClassifierTrees();
		}
	}

	loadClassifierTrees = async () => {
		const { currentCompany } = this.props,
			{ type } = this.state;

		if (currentCompany && type) {
			let trees: IClassifierTree[] = [],
				treeUid: Guid;

			if (type.hierarchyType == "Groups") {
				trees = (await this._classifierTreeService.list(currentCompany.uid, { typeCode: type.code })).rows;

				if (trees && trees.length > 0) {
					treeUid = trees[0].uid;
				}
			}

			this.setState({
				trees,
				treeUid
			});

			await this.loadClassifierGroups();
		}
	}

	loadClassifierGroups = async (focusGroupUid?: Guid) => {
		// to re-render page w/o groups tree, otherwise tree not refreshed
		this.setState({ groups: null });

		const { currentCompany } = this.props,
			{ type, treeUid } = this.state;

		if (currentCompany && type) {
			let groups: IClassifierGroup[] = [];

			if (type.hierarchyType == "Groups") {
				groups = await this.fetchClassifierGroups(type.code, treeUid, null, focusGroupUid, true);
			}

			if (type.hierarchyType == "Items") {
				groups = await this.fetchClassifierGroups(type.code, null, null, focusGroupUid, true);
			}

			this.setState({ groups });
		}
	}

	fetchClassifierGroups = async (typeCode: string, treeUid?: Guid, parentUid?: Guid, focusUid?: Guid, expandSingleChild?: boolean): Promise<IClassifierGroup[]> => {
		const { currentCompany } = this.props

		const result = await this._classifierGroupService.list(
			currentCompany.uid, {
				typeCode,
				treeUid,
				parentUid,
				focusUid,
				expandSingleChild
			});

		return result.rows;
	}

	// todo: move button to separate class?
	delete = async () => {
		const rowsAffected = await this._classifierService
			.delete(this.props.currentCompany.uid,
				this.props.match.params.typeCode,
				this.state.selectedRowKeys);

		this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

		await this.refreshTable();
	}

	// todo: move button to separate class?
	export = async () => {
		// todo: show export dialog: all pages, current page, export format
		await this._classifierService.export(this.props.currentCompany.uid, {
			typeCode: this.props.match.params.typeCode
		});
	}

	onTableSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	}

	onTreeRootSelect = async (value: string) => {
		const { trees } = this.state;

		const tree = trees.find(x => x.code == value);

		if (tree) {
			// https://reactjs.org/docs/react-component.html#setstate - Generally we recommend using componentDidUpdate()
			// todo: rewrite using componentDidUpdate() instead of callback in setState
			this.setState({ treeUid: tree.uid, selectedGroup: null, expandedKeys: [] }, async () => {
				await this.loadClassifierGroups();
				await this.refreshTable();
			});
		}
	}

	onTreeLoadData = async (node: AntTreeNode) => new Promise<void>(async (resolve) => {
		const group: IClassifierGroup = node.props.dataRef;

		if (!group.children) {
			const { type, treeUid, expandedKeys } = this.state;

			const children = await this.fetchClassifierGroups(type.code, treeUid, group.uid, null, true)

			// to populate new expanded keys
			this.buildGroupsTree(children, expandedKeys);

			group.children = children;

			this.setState({
				groups: [...this.state.groups],
				expandedKeys
			});
		}

		resolve();
	})

	onTreeSelect = async (selectedKeys: string[], e: AntTreeNodeSelectedEvent) => {
		this.setState({
			selectedGroup: (e.selected) ? e.node.props.dataRef : null
		});

		await this.refreshTable();
	}

	onTreeExpand = (expandedKeys: string[], e: AntTreeNodeExpandedEvent) => {
		this.setState({ expandedKeys });
	}

	onDepthChange = async (e: RadioChangeEvent) => {
		// todo: store depth in local storage

		this.setState({ depth: e.target.value as string });

		await this.refreshTable();
	}

	buildGroupsTree = (groups: IClassifierGroup[], expanded?: string[]) => {
		return groups && groups.map(x => {

			if (expanded && x.children) {
				expanded.push(x.uid.toString());
			}

			return (
				<Tree.TreeNode title={`${x.code}. ${x.name}`} key={`${x.uid}`} dataRef={x}>
					{x.children && this.buildGroupsTree(x.children, expanded)}
				</Tree.TreeNode>
			);
		});
	}

	showAddGroupModal = () => {
		const { selectedGroup, treeUid } = this.state;

		this.setState({ groupEditData: { parentUid: selectedGroup ? selectedGroup.uid : treeUid } });
	}

	showEditGroupModal = () => {
		const { selectedGroup, treeUid } = this.state;

		this.setState({ groupEditData: { uid: selectedGroup ? selectedGroup.uid : treeUid } });
	}

	showDeleteGroupConfirm = () => {
		Modal.confirm({
			title: "Вы действительно хотите удалить выбранную группу?",
			content: "Дочерние группы и элементы будут перенесены к родительской группе.",
			onOk: this.deleteSelectedGroup
		});
	}

	deleteSelectedGroup = async () => {
		const { currentCompany } = this.props
		const { type, selectedGroup } = this.state;

		await this._classifierGroupService.delete(currentCompany.uid, type.code, selectedGroup.uid);

		this.setState({ selectedGroup: null });

		// todo: select deleted group parent?
		this.refreshTree(selectedGroup.parentUid);
	}

	refreshTree = async (focusGroupUid?: Guid) => {
		await this.loadClassifierGroups(focusGroupUid);
		await this.refreshTable();
	}

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	}

	onGroupModalSuccess = async (data: IClassifierGroup) => {
		const { expandedKeys } = this.state;

		// after group added - expand parent group
		// todo: expand all parent groups (parent can be selected in modal)
		if (data.parentUid) {
			expandedKeys.push(data.parentUid.toString());
		}

		this.setState({ groupEditData: null, selectedGroup: data, expandedKeys });

		await this.refreshTree(data.uid);
	}

	onGroupModalCancel = () => {
		this.setState({ groupEditData: null });
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { currentCompany } = this.props,
			{ type, treeUid, depth, selectedGroup } = this.state;

		if (currentCompany && type.code) {

			const params = {
				companyUid: currentCompany.uid,
				typeCode: type.code,
				treeUid,
				depth,
				groupUid: selectedGroup ? selectedGroup.uid : null,
				...postParams
			};

			return await this._classifierService.post(loadUrl, params);
		}

		return null;
	}

	render() {
		const { currentCompany } = this.props,
			{ types, type, trees, groups, selectedGroup, groupEditData, expandedKeys, updateTableToken } = this.state;

		if (!currentCompany || !type) return null;

		// todo: settings
		// 1. how tree looks - list or tree (?)
		// 2. hide tree
		// 3. show or hide groups in list

		let groupControls;
		if (type.hierarchyType == "Groups") {
			groupControls = <>
				<Select defaultValue="default" size="small" onSelect={this.onTreeRootSelect} style={{ minWidth: 200 }}>
					{trees && trees.map(x => <Select.Option key={x.code}>{x.name || x.code}</Select.Option>)}
				</Select>
				<Button.Group size="small">
					<Button icon="plus" onClick={this.showAddGroupModal} />
					<Button icon="edit" onClick={this.showEditGroupModal} disabled={!selectedGroup} />
					<Button icon="delete" onClick={this.showDeleteGroupConfirm} disabled={!selectedGroup} />
				</Button.Group>
			</>
		}

		let tree;
		if (type.hierarchyType != "None") {
			if (groups) {
				const defaultExpandedKeys: string[] = [],
					selectedKeys: Guid[] = [];

				const nodes = this.buildGroupsTree(groups, defaultExpandedKeys);

				if (selectedGroup) {
					selectedKeys.push(selectedGroup.uid);
				}

				tree = (
					<Tree blockNode
						defaultExpandedKeys={defaultExpandedKeys}
						defaultSelectedKeys={selectedKeys.map(x => x.toString())}
						expandedKeys={expandedKeys}
						loadData={this.onTreeLoadData}
						onSelect={this.onTreeSelect}
						onExpand={this.onTreeExpand}>
						{nodes}
					</Tree>
				);
			}
			else {
				tree = <Spin />;
			}
		}

		const table = (
			<DataTable
				rowKey="uid"
				viewId={`Classifier/Grid/${type.code}`}
				loadUrl={`${Constants.baseURL}/classifier/list/`}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onTableSelectionChange}
				updateToken={updateTableToken}
			/>
		);

		const content = (tree)
			? (<>
				<Layout.Sider width="360" theme="light" collapsible={false} style={{ overflowX: "auto", height: "80vh" }}>
					{tree}
				</Layout.Sider>
				<Layout.Content className="bg-white" style={{ paddingTop: 0, paddingRight: 0 }}>
					{table}
				</Layout.Content>
			</>)
			: (table);

		return (
			<Page
				title={<>
					<Toolbar float="right">
						<Link to={`/classifiers/${type.code}/add`}>
							<Button type="primary"><Icon type="plus" /> Добавить</Button>
						</Link>
						<Button onClick={this.delete}><Icon type="delete" /> Удалить</Button>
						<Button onClick={this.export}><Icon type="export" /> Экспорт</Button>
						<Link to={`/classifiers/edit/${type.uid}`}>
							<Button><Icon type="setting" /> Настройка</Button>
						</Link>
					</Toolbar>

					<ClassifierBreadcrumb type={type} types={types} />
					<PageHeader>{type.name}</PageHeader>
				</>}>

				<Layout>
					<Layout.Header className="bg-white" style={{ padding: "0", lineHeight: 1.5, height: 36 }}>

						<Toolbar size="small">
							{/* <Button size="small" icon="left" /> */}
							{groupControls}
							{/* <Button size="small" icon="search" /> */}
						</Toolbar>

						<Toolbar size="small" float="right">
							{/* <Input.Search size="small" allowClear style={{ width: 200 }} /> */}
							{/* todo: show only when Items or Groups hierarchy type */}
							<Radio.Group defaultValue="0" size="small" onChange={this.onDepthChange}>
								<Radio.Button value="0"><Icon type="folder" /> Группа</Radio.Button>
								<Radio.Button value="1"><Icon type="cluster" /> Иерархия</Radio.Button>
							</Radio.Group>
						</Toolbar>

					</Layout.Header>
					<Layout hasSider={!!tree} className="bg-white">
						{content}
					</Layout>
				</Layout>

				{groupEditData &&
					<ModalEditClassifierGroup
						typeCode={type.code}
						uid={groupEditData.uid}
						parentUid={groupEditData.parentUid}
						onSuccess={this.onGroupModalSuccess}
						onCancel={this.onGroupModalCancel} />}

			</Page>
		);
	}
}

export const SearchClassifier = withCompanyContext(_SearchClassifier);
