import * as React from "react";
import { Page, DataTable, PageHeader, Toolbar } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { Icon, Button, Tree, Select, Radio, Layout, Modal, Spin } from "antd";
import { Constants } from "@montr-core/.";
import { Guid, IDataResult } from "@montr-core/models";
import { NotificationService } from "@montr-core/services";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService, ClassifierTypeService, ClassifierGroupService } from "../services";
import { IClassifierType, IClassifierTree, IClassifierGroup } from "../models";
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
	type: IClassifierType;
	trees?: IClassifierTree[];
	treeCode?: string,
	groups?: IClassifierGroup[];
	selectedGroup?: IClassifierGroup;
	groupEditData?: { parentUid?: Guid, uid?: Guid },
	selectedRowKeys: string[] | number[];
	depth: string;
	// postParams: any;
	updateTableDate?: Date;
}

class _SearchClassifier extends React.Component<IProps, IState> {
	private _classifierTypeService = new ClassifierTypeService();
	private _classifierGroupService = new ClassifierGroupService();
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
			depth: "0",
			/* postParams: {
				depth: "0"
			} */
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
		await this._classifierTypeService.abort();
		await this._classifierGroupService.abort();
		await this._classifierService.abort();
	}

	setPostParams = async () => {
		const { currentCompany } = this.props,
			{ typeCode } = this.props.match.params;

		if (!currentCompany) return;

		const types = (await this._classifierTypeService.list(currentCompany.uid)).rows;
		const type = await this._classifierTypeService.get(currentCompany.uid, { typeCode });

		let trees: IClassifierTree[] = [],
			treeCode: string,
			groups: IClassifierGroup[] = [];

		if (type) {
			if (type.hierarchyType == "Groups") {
				trees = (await this._classifierService.trees(currentCompany.uid, type.code)).rows;

				if (trees && trees.length > 0) {
					treeCode = trees[0].code;
					groups = await this.fetchClassifierGroups(type.code, treeCode);
				}
			}

			if (type.hierarchyType == "Items") {
				groups = await this.fetchClassifierGroups(type.code, null);
			}

			this.setState({
				types: types,
				type: type,
				trees: trees,
				treeCode: treeCode,
				groups: groups,
				/* postParams: {
					companyUid: currentCompany ? currentCompany.uid : null,
					typeCode: typeCode,
					treeCode: treeCode
				}, */
				// updateTableDate: new Date()
			});

			await this.refreshTable();
		}
	}

	fetchClassifierGroups = async (typeCode: string, treeCode: string, parentUid?: Guid, focusUid?: Guid): Promise<IClassifierGroup[]> => {
		const { currentCompany } = this.props

		return await this._classifierGroupService.list(currentCompany.uid, { typeCode, treeCode, parentUid, focusUid });
	}

	// todo: move button to separate class?
	delete = async () => {
		const rowsAffected = await this._classifierService
			.delete(this.props.currentCompany.uid,
				this.props.match.params.typeCode,
				this.state.selectedRowKeys);

		this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

		// this.setPostParams(); // to force table refresh
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

	onTreeLoadData = async (node: AntTreeNode) => new Promise(async (resolve) => {
		const group: IClassifierGroup = node.props.dataRef;

		const { type, treeCode } = this.state;

		const children = await this.fetchClassifierGroups(type.code, treeCode, group.uid)

		group.children = children;

		this.setState({
			groups: [...this.state.groups],
		});

		resolve();
	})

	onTreeSelect = async (selectedKeys: string[], e: AntTreeNodeSelectedEvent) => {
		// const { postParams } = this.state;

		this.setState({
			selectedGroup: (e.selected) ? e.node.props.dataRef : null,
			// postParams: { ...postParams, groupCode: selectedKeys[0] },
			// updateTableDate: new Date()
		});

		await this.refreshTable();
	}

	onTreeExpand = (expandedKeys: string[], e: AntTreeNodeExpandedEvent) => {
		// todo: save all expanded kes before refresh tree?
	}

	onDepthChange = async (e: RadioChangeEvent) => {
		// const { postParams } = this.state;

		this.setState({
			depth: e.target.value,
			// postParams: { ...postParams, depth: e.target.value },
			// updateTableDate: new Date()
		});

		await this.refreshTable();
	}

	buildGroupsTree = (groups: IClassifierGroup[], expanded?: Guid[]) => {
		return groups && groups.map(x => {

			if (expanded && x.children) {
				expanded.push(x.uid);
			}

			return (
				<Tree.TreeNode title={`${x.code}. ${x.name}`} key={`${x.uid}`} dataRef={x}>
					{x.children && this.buildGroupsTree(x.children, expanded)}
				</Tree.TreeNode>
			);
		});
	}

	showAddGroupModal = () => {
		const { selectedGroup } = this.state;

		this.setState({ groupEditData: { parentUid: selectedGroup ? selectedGroup.uid : null } });
	}

	showEditGroupModal = () => {
		const { selectedGroup } = this.state;

		this.setState({ groupEditData: { uid: selectedGroup ? selectedGroup.uid : null } });
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
		const { type, treeCode, selectedGroup } = this.state;

		await this._classifierGroupService.delete(currentCompany.uid, type.code, treeCode, selectedGroup.uid);

		this.setState({ selectedGroup: null });

		// todo: select deleted group parent?
		this.refreshTree(selectedGroup.parentUid);
	}

	refreshTree = async (focusUid?: Guid) => {
		// to re-render page w/o groups tree, otherwise tree not refreshed
		this.setState({ groups: null });

		const { type, trees, treeCode } = this.state;

		if (type) {
			let groups: IClassifierGroup[] = [];

			if (type.hierarchyType == "Groups") {
				if (trees && trees.length > 0) {
					groups = await this.fetchClassifierGroups(type.code, treeCode, null, focusUid);
				}
			}

			if (type.hierarchyType == "Items") {
				groups = await this.fetchClassifierGroups(type.code, null, null, focusUid);
			}

			this.setState({ groups });

			await this.refreshTable();
		}
	}

	refreshTable = async () => {
		this.setState({
			updateTableDate: new Date()
		});
	}

	onGroupModalSuccess = async (data: IClassifierGroup) => {
		this.setState({ groupEditData: null, selectedGroup: data });

		await this.refreshTree(data.uid);
	}

	onGroupModalCancel = () => {
		this.setState({ groupEditData: null });
	}

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { currentCompany } = this.props,
			{ type, treeCode, depth, selectedGroup } = this.state;

		if (currentCompany && type.code) {

			const params = {
				companyUid: currentCompany.uid,
				typeCode: type.code,
				treeCode,
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
			{ types, type, treeCode, trees, groups, selectedGroup, groupEditData, /* postParams, */ updateTableDate } = this.state;

		if (!currentCompany || !type /* || !postParams.typeCode */) return null;

		// todo: настройки:
		// 1. как выглядит дерево - списком или деревом (?)
		// 2. прятать дерево
		// 3. показывать или нет группы в таблице
		// 4. показывать планарную таблицу без групп

		let groupControls;
		if (type.hierarchyType == "Groups") {
			groupControls = <>
				<Select defaultValue="default" size="small">
					{trees.map(x => <Select.Option key={x.code}>{x.name || x.code}</Select.Option>)}
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
				const expandedKeys: Guid[] = [],
					selectedKeys: Guid[] = [];

				const nodes = this.buildGroupsTree(groups, expandedKeys);

				if (selectedGroup) {
					selectedKeys.push(selectedGroup.uid);
				}

				tree = (
					<Tree blockNode
						defaultExpandedKeys={expandedKeys.map(x => x.toString())}
						defaultSelectedKeys={selectedKeys.map(x => x.toString())}
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
				updateDate={updateTableDate}
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
							<Radio.Group defaultValue="0" size="small" onChange={this.onDepthChange}>
								<Radio.Button value="0"><Icon type="folder" /> Группа</Radio.Button>
								<Radio.Button value="1"><Icon type="cluster" /> Иерархия</Radio.Button>
							</Radio.Group>
							<Link to={`/classifiers/edit/${type.uid}`}>
								<Button size="small"><Icon type="setting" /></Button>
							</Link>
						</Toolbar>

					</Layout.Header>
					<Layout hasSider={!!tree} className="bg-white">
						{content}
					</Layout>
				</Layout>

				{groupEditData &&
					<ModalEditClassifierGroup
						typeCode={type.code}
						treeCode={treeCode}
						uid={groupEditData.uid}
						parentUid={groupEditData.parentUid}
						onSuccess={this.onGroupModalSuccess}
						onCancel={this.onGroupModalCancel} />}

			</Page>
		);
	}
}

export const SearchClassifier = withCompanyContext(_SearchClassifier);
