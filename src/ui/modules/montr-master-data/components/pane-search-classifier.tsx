import { ButtonAdd, ButtonDelete, ButtonExport, ButtonSelect, DataTable, DataTableUpdateToken, Icon, Page, PageHeader, Toolbar } from "@montr-core/components";
import { DataResult, Guid } from "@montr-core/models";
import { OperationService } from "@montr-core/services";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components/company-context";
import { Button, Layout, Modal, Radio, Select, Spin, Tree } from "antd";
import { RadioChangeEvent } from "antd/lib/radio";
import { DataNode, EventDataNode } from "antd/lib/tree";
import * as React from "react";
import { Translation } from "react-i18next";
import { Link } from "react-router-dom";
import { ClassifierBreadcrumb, ModalEditClassifierGroup, PaneEditClassifier } from "../components";
import { Classifier, ClassifierGroup, ClassifierTree, ClassifierType } from "../models";
import { Api, Locale, RouteBuilder, Views } from "../module";
import { ClassifierGroupService, ClassifierService, ClassifierTreeService, ClassifierTypeService } from "../services";

interface Props extends CompanyContextProps {
	typeCode: string;
	mode: "page" | "drawer";
	onSelect?: (keys: string[] | number[], rows: any[]) => Promise<void>;
}

interface State {
	types: ClassifierType[];
	type?: ClassifierType;
	trees?: ClassifierTree[];
	groups?: ClassifierGroup[];
	selectedTree?: ClassifierTree,
	selectedGroup?: ClassifierGroup;
	groupEditData?: ClassifierGroup;
	showEditPane?: boolean;
	editUid?: Guid;
	expandedKeys: string[];
	selectedRowKeys: string[] | number[];
	selectedRows: Classifier[];
	depth: string; // todo: make enum
	updateTableToken: DataTableUpdateToken;
}

interface TreeNode extends EventDataNode<ClassifierGroup> {
	dataRef: ClassifierGroup;
}

interface TreeNodeSelectEvent {
	// event: 'select';
	selected: boolean;
	node: EventDataNode<ClassifierGroup>;
	// selectedNodes: TreeDataType[];
	// nativeEvent: MouseEvent;
}

class WrappedPaneSearchClassifier extends React.Component<Props, State> {

	private readonly operation = new OperationService();
	private readonly classifierTypeService = new ClassifierTypeService();
	private readonly classifierTreeService = new ClassifierTreeService();
	private readonly classifierGroupService = new ClassifierGroupService();
	private readonly classifierService = new ClassifierService();

	constructor(props: Props) {
		super(props);

		this.state = {
			types: [],
			expandedKeys: [],
			selectedRowKeys: [],
			selectedRows: [],
			depth: "0",
			updateTableToken: { date: new Date() }
		};
	}

	componentDidMount = async () => {
		await this.loadClassifierTypes();
		// await this.loadClassifierType();
	};

	componentDidUpdate = async (prevProps: Props) => {
		// todo: remove after current company will be selected on separate page
		if (this.props.currentCompany !== prevProps.currentCompany) {
			// todo: check if selected type belongs to company (show 404)
			await this.loadClassifierTypes();
			// await this.loadClassifierType();
		}
		else if (this.props.typeCode !== prevProps.typeCode) {

			this.setState({
				selectedRowKeys: [],
				selectedGroup: null
			});

			await this.loadClassifierType();
			await this.refreshTable(true, true);
		}
	};

	componentWillUnmount = async () => {
		await this.classifierTypeService.abort();
		await this.classifierTreeService.abort();
		await this.classifierGroupService.abort();
		await this.classifierService.abort();
	};

	loadClassifierTypes = async () => {
		const result = await this.classifierTypeService.list({ skipPaging: true });

		if (result) {
			this.setState({ types: result.rows }, async () => await this.loadClassifierType());
		}
	};

	loadClassifierType = async () => {
		const { typeCode } = this.props;

		const type = await this.classifierTypeService.get({ typeCode });

		this.setState({ type }, async () => await this.loadClassifierTrees());
	};

	loadClassifierTrees = async () => {
		const { type } = this.state;

		if (type) {
			let trees: ClassifierTree[] = [],
				selectedTree: ClassifierTree;

			if (type.hierarchyType == "Groups") {
				trees = (await this.classifierTreeService.list({ typeCode: type.code })).rows;

				if (trees && trees.length > 0) {
					selectedTree = trees.find(x => x.code == "default");
				}
			}

			this.setState({
				trees,
				selectedTree
			}, async () => await this.loadClassifierGroups());
		}
	};

	loadClassifierGroups = async (focusGroupUid?: Guid) => {
		// to re-render page w/o groups tree, otherwise tree not refreshed
		this.setState({ groups: null });

		const { type, selectedTree } = this.state;

		if (type) {
			let groups: ClassifierGroup[] = [];

			if (type.hierarchyType == "Groups" && selectedTree) {
				groups = await this.fetchClassifierGroups(type.code, selectedTree.uid, null, focusGroupUid, true);
			}

			if (type.hierarchyType == "Items") {
				groups = await this.fetchClassifierGroups(type.code, null, null, focusGroupUid, true);
			}

			this.setState({ groups });
		}
	};

	fetchClassifierGroups = async (typeCode: string, treeUid?: Guid, parentUid?: Guid, focusUid?: Guid, expandSingleChild?: boolean): Promise<ClassifierGroup[]> => {
		const result = await this.classifierGroupService.list({
			typeCode,
			treeUid,
			parentUid,
			focusUid,
			expandSingleChild
		});

		return result.rows;
	};

	// todo: move button to separate class?
	delete = async () => {
		await this.operation.confirmDelete(async () => {
			const result = await this.classifierService
				.delete(this.props.typeCode, this.state.selectedRowKeys);

			if (result.success) {
				await this.refreshTable(true, true);
			}

			return result;
		});
	};

	// todo: move button to separate class?
	export = async () => {
		// todo: show export dialog: all pages, current page, export format
		await this.classifierService.export({ typeCode: this.props.typeCode });
	};

	onTableSelectionChange = async (selectedRowKeys: string[] | number[], selectedRows: any[]) => {
		this.setState({ selectedRowKeys, selectedRows });
	};

	onTreeSelect = async (value: string) => {
		const { trees } = this.state;

		const selectedTree = trees.find(x => x.code == value);

		if (selectedTree) {
			// https://reactjs.org/docs/react-component.html#setstate - Generally we recommend using componentDidUpdate()
			// todo: rewrite using componentDidUpdate() instead of callback in setState
			this.setState({ selectedTree, selectedGroup: null, expandedKeys: [] }, async () => {
				await this.loadClassifierGroups();
				await this.refreshTable();
			});
		}
	};

	onTreeLoadData = async (node: TreeNode) => {
		return new Promise<void>(async (resolve) => {
			const group: ClassifierGroup = node.dataRef;

			if (!group.children) {
				const { type, selectedTree, expandedKeys } = this.state;

				const treeUid = selectedTree ? selectedTree.uid : null;

				const children = await this.fetchClassifierGroups(type.code, treeUid, group.uid, null, true);

				// todo: refactor - only to populate new expanded keys parameter
				this.buildGroupsTree(children, expandedKeys);

				group.children = children;

				this.setState({
					groups: [...this.state.groups],
					expandedKeys
				});
			}

			resolve();
		});
	};

	onTreeNodeSelect = async (selectedKeys: string[], e: TreeNodeSelectEvent) => {
		const node = e.node as TreeNode;
		this.setState({
			selectedGroup: (e.selected) ? node?.dataRef : null
		}, async () => await this.refreshTable());
	};

	onTreeNodeExpand = (expandedKeys: string[]) => {
		this.setState({ expandedKeys });
	};

	onDepthChange = async (e: RadioChangeEvent) => {
		// todo: store depth in local storage

		this.setState({ depth: e.target.value as string });

		await this.refreshTable();
	};

	buildGroupsTree = (groups: ClassifierGroup[], expanded?: string[]): DataNode[] => {
		return groups && groups.map(x => {

			if (expanded && x.children) {
				expanded.push(x.uid.toString());
			}

			// todo: convert to component (?)
			const result = {
				title: <span><span style={{ color: "silver" }}>{x.code}</span> {x.name}</span>,
				key: `${x.uid}`,
				dataRef: x,
				children: x.children && this.buildGroupsTree(x.children, expanded)
			};

			return result;
		});
	};

	showAddGroupModal = () => {
		const { selectedGroup } = this.state;

		this.setState({ groupEditData: { parentUid: selectedGroup ? selectedGroup.uid : null } });
	};

	showEditGroupModal = () => {
		const { selectedGroup } = this.state;

		if (selectedGroup) {
			this.setState({ groupEditData: { uid: selectedGroup.uid } });
		}
	};

	showDeleteGroupConfirm = () => {
		Modal.confirm({
			title: "Вы действительно хотите удалить выбранную группу?",
			content: "Дочерние группы и элементы будут перенесены к родительской группе.",
			onOk: this.deleteSelectedGroup
		});
	};

	deleteSelectedGroup = async () => {
		const { type, selectedGroup } = this.state;

		await this.classifierGroupService.delete(type.code, selectedGroup.uid);

		this.setState({ selectedGroup: null });

		// todo: select deleted group parent?
		this.refreshTree(selectedGroup.parentUid);
	};

	refreshTree = async (focusGroupUid?: Guid) => {
		await this.loadClassifierGroups(focusGroupUid);
		await this.refreshTable();
	};

	refreshTable = (resetCurrentPage?: boolean, resetSelectedRows?: boolean): void => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	onGroupModalSuccess = async (data: ClassifierGroup) => {
		const { expandedKeys } = this.state;

		// after group added - expand parent group
		// todo: expand all parent groups (parent can be selected in modal)
		if (data.parentUid) {
			expandedKeys.push(data.parentUid.toString());
		}

		this.setState({ groupEditData: null, selectedGroup: data, expandedKeys });

		await this.refreshTree(data.uid);
	};

	onGroupModalCancel = () => {
		this.setState({ groupEditData: null });
	};

	onTableLoadData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {
		const { currentCompany } = this.props,
			{ type, selectedTree, selectedGroup, depth } = this.state;

		if (currentCompany && type.code) {

			const params = {
				companyUid: currentCompany.uid,
				typeCode: type.code,
				treeUid: selectedTree ? selectedTree.uid : null,
				groupUid: selectedGroup ? selectedGroup.uid : null,
				depth,
				...postParams
			};

			return await this.classifierService.post(loadUrl, params);
		}

		return null;
	};

	showAddPane = () => {
		this.setState({ showEditPane: true, editUid: null });
	};

	showEditPane = (data: Classifier) => {
		this.setState({ showEditPane: true, editUid: data?.uid });
	};

	closeEditPane = () => {
		this.setState({ showEditPane: false });
	};

	handleEditPaneSuccess = () => {
		this.setState({ showEditPane: false });
		this.refreshTable();
	};

	select = () => {
		const { onSelect } = this.props,
			{ selectedRowKeys, selectedRows } = this.state;

		if (onSelect) {
			onSelect(selectedRowKeys, selectedRows);
		}
	};

	render = (): React.ReactNode => {
		const { mode, currentCompany } = this.props,
			{ types, type, trees, groups, selectedTree, selectedGroup, groupEditData, expandedKeys,
				updateTableToken, selectedRowKeys,
				showEditPane, editUid } = this.state;

		if (!currentCompany || !type) return null;

		// todo: settings
		// 1. how tree looks - list or tree (?)
		// 2. hide tree
		// 3. show or hide groups in list

		const selectedGroupUid = selectedGroup ? selectedGroup.uid : null;

		let groupControls;
		if (type.hierarchyType == "Groups") {
			groupControls = <>
				<Select defaultValue="default" size="small" onSelect={this.onTreeSelect} style={{ minWidth: 200 }}>
					{trees && trees.map(x => <Select.Option key={x.code} value={x.code}>{x.name || x.code}</Select.Option>)}
				</Select>
				<Button.Group size="small">
					<Button icon={Icon.Plus} onClick={this.showAddGroupModal} />
					<Button icon={Icon.Edit} onClick={this.showEditGroupModal} disabled={!selectedGroup} />
					<Button icon={Icon.Delete} onClick={this.showDeleteGroupConfirm} disabled={!selectedGroup} />
				</Button.Group>
			</>;
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
						treeData={nodes}
						defaultExpandedKeys={defaultExpandedKeys}
						defaultSelectedKeys={selectedKeys.map(x => x.toString())}
						expandedKeys={expandedKeys}
						loadData={this.onTreeLoadData}
						onSelect={this.onTreeNodeSelect}
						onExpand={this.onTreeNodeExpand} />
				);
			}
			else {
				tree = <Spin />;
			}
		}

		const table = (<Translation ns={Locale.Namespace}>{(t) => <>
			<DataTable
				rowKey="uid"
				viewId={Views.classifierList}
				loadUrl={Api.classifierList}
				onLoadData={this.onTableLoadData}
				onSelectionChange={this.onTableSelectionChange}
				rowActions={[{ name: t("button.edit"), onClick: this.showEditPane }]}
				updateToken={updateTableToken}
			/>
		</>}</Translation>);

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
						{/* <Link to={RouteBuilder.addClassifier(type.code, selectedGroupUid)}>
							<ButtonAdd type="primary" />
						</Link> */}
						<ButtonAdd type="primary" onClick={this.showAddPane} />
						<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
						<ButtonExport onClick={this.export} />
						<Link to={RouteBuilder.editClassifierType(type.uid)}>
							<Button icon={Icon.Setting}> Настроить</Button>
						</Link>
					</Toolbar>

					{mode == "page" && <>
						<ClassifierBreadcrumb type={type} types={types} />
						<PageHeader>{type.name}</PageHeader>
					</>}

					{mode == "drawer" && <Toolbar clear>
						<ButtonSelect type="primary" onClick={this.select} />
					</Toolbar>}

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
								<Radio.Button value="0">{Icon.Folder} Группа</Radio.Button>
								<Radio.Button value="1">{Icon.Cluster} Иерархия</Radio.Button>
							</Radio.Group>
						</Toolbar>

					</Layout.Header>
					<Layout hasSider={!!tree} className="bg-white">
						{content}
					</Layout>
				</Layout>

				{groupEditData && selectedTree &&
					<ModalEditClassifierGroup
						typeCode={type.code}
						treeUid={selectedTree.uid}
						uid={groupEditData.uid}
						parentUid={groupEditData.parentUid}
						onSuccess={this.onGroupModalSuccess}
						onCancel={this.onGroupModalCancel} />}

				{showEditPane &&
					<PaneEditClassifier
						type={type}
						uid={editUid}
						parentUid={selectedGroupUid}
						onSuccess={this.handleEditPaneSuccess}
						onClose={this.closeEditPane}
					/>
				}

			</Page>
		);
	};
}

export const PaneSearchClassifier = withCompanyContext(WrappedPaneSearchClassifier);
