import * as React from "react";
import { Page, PageHeader, Toolbar, DataTable, DataTableUpdateToken, ButtonAdd, ButtonDelete, ButtonExport, Icon } from "@montr-core/components";
import { Link } from "react-router-dom";
import { Button, Tree, Select, Radio, Layout, Modal, Spin } from "antd";
import { TreeNodeNormal } from "antd/lib/tree/Tree";
import { EventDataNode } from "rc-tree/lib/interface";
import { RadioChangeEvent } from "antd/lib/radio";
import { Guid, DataResult } from "@montr-core/models";
import { NotificationService } from "@montr-core/services";
import { withCompanyContext, CompanyContextProps } from "@montr-kompany/components";
import { ClassifierService, ClassifierTypeService, ClassifierGroupService, ClassifierTreeService } from "../services";
import { IClassifierType, IClassifierGroup, IClassifierTree } from "../models";
import { ClassifierBreadcrumb, ModalEditClassifierGroup } from "../components";
import { RouteBuilder, Api, Views } from "../module";

interface IProps extends CompanyContextProps {
	typeCode: string;
	mode: "Page" | "Drawer";
	onSelect?: (keys: string[] | number[]) => void;
}

interface IState {
	types: IClassifierType[];
	type?: IClassifierType;
	trees?: IClassifierTree[];
	groups?: IClassifierGroup[];
	selectedTree?: IClassifierTree,
	selectedGroup?: IClassifierGroup;
	groupEditData?: IClassifierGroup;
	expandedKeys: string[];
	selectedRowKeys: string[] | number[];
	depth: string; // todo: make enum
	updateTableToken: DataTableUpdateToken;
}

interface ITreeNode extends EventDataNode /* AntTreeNode */ {
	dataRef: IClassifierGroup;
}

interface ITreeNodeSelectEvent {
	// event: 'select';
	selected: boolean;
	node: EventDataNode;
	// selectedNodes: DataNode[];
	nativeEvent: MouseEvent;
}

interface ITreeNodeExpandEvent {
	node: EventDataNode;
	expanded: boolean;
	nativeEvent: MouseEvent;
}

class _PaneSearchClassifier extends React.Component<IProps, IState> {

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
		// await this.loadClassifierType();
	};

	componentDidUpdate = async (prevProps: IProps) => {
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
			await this.refreshTable(true);
		}
	};

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
		await this._classifierTreeService.abort();
		await this._classifierGroupService.abort();
		await this._classifierService.abort();
	};

	loadClassifierTypes = async () => {
		const types = (await this._classifierTypeService.list({ skipPaging: true })).rows;

		this.setState({ types });

		await this.loadClassifierType();
	};

	loadClassifierType = async () => {
		const { typeCode } = this.props;

		const type = await this._classifierTypeService.get({ typeCode });

		this.setState({ type });

		await this.loadClassifierTrees();
	};

	loadClassifierTrees = async () => {
		const { type } = this.state;

		if (type) {
			let trees: IClassifierTree[] = [],
				selectedTree: IClassifierTree;

			if (type.hierarchyType == "Groups") {
				trees = (await this._classifierTreeService.list({ typeCode: type.code })).rows;

				if (trees && trees.length > 0) {
					selectedTree = trees.find(x => x.code == "default");
				}
			}

			this.setState({
				trees,
				selectedTree
			});

			await this.loadClassifierGroups();
		}
	};

	loadClassifierGroups = async (focusGroupUid?: Guid) => {
		// to re-render page w/o groups tree, otherwise tree not refreshed
		this.setState({ groups: null });

		const { type, selectedTree } = this.state;

		if (type) {
			let groups: IClassifierGroup[] = [];

			if (type.hierarchyType == "Groups" && selectedTree) {
				groups = await this.fetchClassifierGroups(type.code, selectedTree.uid, null, focusGroupUid, true);
			}

			if (type.hierarchyType == "Items") {
				groups = await this.fetchClassifierGroups(type.code, null, null, focusGroupUid, true);
			}

			this.setState({ groups });
		}
	};

	fetchClassifierGroups = async (typeCode: string, treeUid?: Guid, parentUid?: Guid, focusUid?: Guid, expandSingleChild?: boolean): Promise<IClassifierGroup[]> => {
		const result = await this._classifierGroupService.list({
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
		const rowsAffected = await this._classifierService
			.delete(this.props.typeCode, this.state.selectedRowKeys);

		this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

		await this.refreshTable();
	};

	// todo: move button to separate class?
	export = async () => {
		// todo: show export dialog: all pages, current page, export format
		await this._classifierService.export({ typeCode: this.props.typeCode });
	};

	onTableSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
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

	onTreeLoadData = async (node: ITreeNode /* AntTreeNode */) => {
		return new Promise<void>(async (resolve) => {
			const group: IClassifierGroup = node.dataRef;

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

	onTreeNodeSelect = async (selectedKeys: string[], e: ITreeNodeSelectEvent /* AntTreeNodeSelectedEvent */) => {
		const node = e.node as ITreeNode;
		this.setState({
			selectedGroup: (e.selected) ? node?.dataRef : null
		}, async () => await this.refreshTable());
	};

	onTreeExpand = (expandedKeys: string[], e: ITreeNodeExpandEvent /* AntTreeNodeExpandedEvent */) => {
		this.setState({ expandedKeys });
	};

	onDepthChange = async (e: RadioChangeEvent) => {
		// todo: store depth in local storage

		this.setState({ depth: e.target.value as string });

		await this.refreshTable();
	};

	buildGroupsTree = (groups: IClassifierGroup[], expanded?: string[]): TreeNodeNormal[] => {
		return groups && groups.map(x => {

			if (expanded && x.children) {
				expanded.push(x.uid.toString());
			}

			// todo: convert to component (?)
			return {
				title: <span><span style={{ color: "silver" }}>{x.code}</span> {x.name}</span>,
				key: `${x.uid}`,
				dataRef: x,
				children: x.children && this.buildGroupsTree(x.children, expanded)
			};
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

		await this._classifierGroupService.delete(type.code, selectedGroup.uid);

		this.setState({ selectedGroup: null });

		// todo: select deleted group parent?
		this.refreshTree(selectedGroup.parentUid);
	};

	refreshTree = async (focusGroupUid?: Guid) => {
		await this.loadClassifierGroups(focusGroupUid);
		await this.refreshTable();
	};

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	onGroupModalSuccess = async (data: IClassifierGroup) => {
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

			return await this._classifierService.post(loadUrl, params);
		}

		return null;
	};

	select = () => {
		const { onSelect } = this.props,
			{ selectedRowKeys } = this.state;

		if (onSelect) {
			onSelect(selectedRowKeys);
		}
	};

	render() {
		const { mode, currentCompany } = this.props,
			{ types, type, trees, groups, selectedTree, selectedGroup, groupEditData, expandedKeys, updateTableToken } = this.state;

		if (!currentCompany || !type) return null;

		// todo: settings
		// 1. how tree looks - list or tree (?)
		// 2. hide tree
		// 3. show or hide groups in list

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
						onExpand={this.onTreeExpand} />
				);
			}
			else {
				tree = <Spin />;
			}
		}

		const table = (
			<DataTable
				rowKey="uid"
				viewId={Views.classifierList}
				loadUrl={Api.classifierList}
				onLoadData={this.onTableLoadData}
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
						<Link to={RouteBuilder.addClassifier(type.code, selectedGroup ? selectedGroup.uid : null)}>
							<ButtonAdd type="primary" />
						</Link>
						<ButtonDelete onClick={this.delete} />
						<ButtonExport onClick={this.export} />
						<Link to={RouteBuilder.editClassifierType(type.uid)}>
							<Button icon={Icon.Setting}> Настроить</Button>
						</Link>
					</Toolbar>

					{mode == "Page" && <>
						<ClassifierBreadcrumb type={type} types={types} />
						<PageHeader>{type.name}</PageHeader>
					</>}

					{mode == "Drawer" && <Toolbar clear>
						<Button type="primary" icon={Icon.Select} onClick={this.select}>Выбрать</Button>
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

			</Page>
		);
	}
}

export const PaneSearchClassifier = withCompanyContext(_PaneSearchClassifier);
