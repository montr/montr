import * as React from "react";
import { Page, DataTable, PageHeader, Toolbar } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { Icon, Button, Tree, Select, Radio, Layout, Modal } from "antd";
import { Constants } from "@montr-core/.";
import { Guid } from "@montr-core/models";
import { NotificationService } from "@montr-core/services";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierService, ClassifierTypeService, ClassifierGroupService } from "../services";
import { IClassifierType, IClassifierTree, IClassifierGroup } from "../models";
import { RadioChangeEvent } from "antd/lib/radio";
import { AntTreeNode, AntTreeNodeSelectedEvent } from "antd/lib/tree";
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
	selectedGroupUid?: Guid;
	groupEditData?: { parentUid?: Guid, uid?: Guid },
	selectedRowKeys: string[] | number[];
	postParams: any;
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
				trees = (await this._classifierService.trees(currentCompany.uid, typeCode)).rows;

				if (trees && trees.length > 0) {
					treeCode = trees[0].code;
					groups = await this.fetchClassifierGroups(typeCode, treeCode);
				}
			}

			if (type.hierarchyType == "Items") {
				groups = await this.fetchClassifierGroups(typeCode, null);
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

	fetchClassifierGroups = async (typeCode: string, treeCode: string, parentCode?: string): Promise<IClassifierGroup[]> => {
		const { currentCompany } = this.props

		return await this._classifierGroupService.list(currentCompany.uid, { typeCode, treeCode, parentCode });
	}

	delete = async () => {
		const rowsAffected = await this._classifierService
			.delete(this.props.currentCompany.uid,
				this.props.match.params.typeCode,
				this.state.selectedRowKeys);

		this._notificationService.success("Выбранные записи удалены. " + rowsAffected);

		this.setPostParams(); // to force table refresh
	}

	export = async () => {
		// todo: show export dialog: all pages, current page, export format
		await this._classifierService.export(this.props.currentCompany.uid, {
			typeCode: this.props.match.params.typeCode
		});
	}

	onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	}

	onTreeLoadData = async (node: AntTreeNode) => new Promise(async (resolve) => {
		const group: IClassifierGroup = node.props.dataRef;

		const { type, treeCode } = this.state;

		const children = await this.fetchClassifierGroups(type.code, treeCode, group.code)

		group.children = children;

		this.setState({
			groups: [...this.state.groups],
		});

		resolve();
	})

	onTreeSelect = (selectedKeys: string[], e: AntTreeNodeSelectedEvent) => {
		const { postParams } = this.state;
		this.setState({
			selectedGroupUid: (e.selected) ? e.node.props.dataRef.uid : null,
			postParams: { ...postParams, groupCode: selectedKeys[0] }
		});
	}

	onDepthChange = (e: RadioChangeEvent) => {
		const { postParams } = this.state;
		this.setState({ postParams: { ...postParams, depth: e.target.value } });
	}

	buildGroupsTree = (groups: IClassifierGroup[]) => {
		return groups && groups.map(x => {
			return (
				<Tree.TreeNode title={`${x.code}. ${x.name}`} key={x.code} dataRef={x}>
					{x.children && this.buildGroupsTree(x.children)}
				</Tree.TreeNode>
			);
		});
	}

	showAddGroupModal = () => {
		this.setState({ groupEditData: { parentUid: this.state.selectedGroupUid } });
	}

	showEditGroupModal = () => {
		this.setState({ groupEditData: { uid: this.state.selectedGroupUid } });
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
		const { type, treeCode, selectedGroupUid } = this.state;

		await this._classifierGroupService.delete(currentCompany.uid, type.code, treeCode, selectedGroupUid);
		this.setState({ selectedGroupUid: null });
		this.refreshTree();
	}

	refreshTree = () => {
		// todo: refresh only tree and focus on inserted/updated item
		this.setPostParams(); // to force refresh
	}

	handleGroupModalSuccess = () => {
		this.setState({ groupEditData: null });

		this.refreshTree();
	}

	hideGroupModal = () => {
		this.setState({ groupEditData: null });
	}

	render() {
		const { currentCompany } = this.props,
			{ types, type, treeCode, trees, groups, selectedGroupUid, groupEditData, postParams } = this.state;

		if (!currentCompany || !type || !postParams.typeCode) return null;

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
					<Button icon="edit" onClick={this.showEditGroupModal} disabled={!selectedGroupUid} />
					<Button icon="delete" onClick={this.showDeleteGroupConfirm} disabled={!selectedGroupUid} />
				</Button.Group>
			</>
		}

		let tree;
		if (type.hierarchyType != "None" /* && groups.length > 0 */) {
			tree = (
				<Tree blockNode
					loadData={this.onTreeLoadData}
					onSelect={this.onTreeSelect}>
					{this.buildGroupsTree(groups)}
				</Tree>
			);
		}

		const table = (
			<DataTable
				viewId={`Classifier/Grid/${type.code}`}
				loadUrl={`${Constants.baseURL}/classifier/list/`}
				postParams={postParams}
				rowKey="uid"
				onSelectionChange={this.onSelectionChange}
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
						onSuccess={this.handleGroupModalSuccess}
						onCancel={this.hideGroupModal} />}

			</Page>
		);
	}
}

export const SearchClassifier = withCompanyContext(_SearchClassifier);
