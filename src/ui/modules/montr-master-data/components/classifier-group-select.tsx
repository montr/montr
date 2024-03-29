import { Icon } from "@montr-core/components";
import { Guid } from "@montr-core/models";
import { Select, Spin, TreeSelect } from "antd";
import { LegacyDataNode } from "rc-tree-select/lib/interface";
import * as React from "react";
import { ClassifierGroup, ClassifierTree, ClassifierType, IClassifierGroupField } from "../models";
import { ClassifierGroupService, ClassifierTreeService, ClassifierTypeService } from "../services";

interface Props {
	// mode: "Tree" | "Group";
	value?: string;
	field: IClassifierGroupField;
	onChange?: (value: any) => void;
}

interface State {
	loading: boolean;
	value: string;
	type?: ClassifierType;
	trees?: ClassifierTree[];
	groups?: ClassifierGroup[];
	expanded: Guid[];
}

// http://ant.design/components/form/?locale=en-US#components-form-demo-customized-form-controls
// https://github.com/ant-design/ant-design/blob/master/components/form/demo/customized-form-controls.md
// todo: rewrite to functional component (see link above)
export class ClassifierGroupSelect extends React.Component<Props, State> {

	/* static getDerivedStateFromProps(nextProps: any) {
		// Should be a controlled component.
		console.log("_ClassifierGroupSelect.getDerivedStateFromProps", arguments);
		if ("value" in nextProps) {
			return nextProps.value ?? null;
		}
		return null;
	} */

	private _classifierTypeService = new ClassifierTypeService();
	private _classifierTreeService = new ClassifierTreeService();
	private _classifierGroupService = new ClassifierGroupService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
			value: props.value,
			expanded: []
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.value !== prevProps.value) {
			// await this.fetchData();
		}
	};

	componentWillUnmount = async (): Promise<void> => {
		await this._classifierTypeService.abort();
		await this._classifierTreeService.abort();
		await this._classifierGroupService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { field } = this.props,
			{ value, expanded } = this.state;

		const type = await this._classifierTypeService.get({ typeCode: field.props.typeCode });

		let trees: ClassifierTree[], groups: ClassifierGroup[];

		if (type.hierarchyType == "Groups") {
			if (field.props.treeCode || field.props.treeUid) {
				const result = await this._classifierGroupService.list({
					typeCode: field.props.typeCode,
					treeCode: field.props.treeCode,
					treeUid: field.props.treeUid,
					focusUid: value
				});

				groups = result.rows;

				await this.collectExpanded(groups, expanded);
			}
			else {
				const result = await this._classifierTreeService.list({ typeCode: field.props.typeCode });

				trees = result.rows;
			}
		}
		else if (type.hierarchyType == "Items") {
			const result = await this._classifierGroupService.list(
				{ typeCode: field.props.typeCode, focusUid: value });

			groups = result.rows;

			await this.collectExpanded(groups, expanded);
		}

		this.setState({ loading: false, type, trees, groups, expanded });
	};

	async collectExpanded(groups: ClassifierGroup[], expanded?: Guid[]): Promise<void> {
		groups && groups.forEach(group => {
			if (group.children) {

				expanded.push(group.uid);

				this.collectExpanded(group.children, expanded);
			}
		});
	}

	handleChange = (value: any, label: any, extra: any): void => {
		const { onChange } = this.props;

		if (onChange) {
			onChange(value);
		}
	};

	onSearch = (value: string): void => {
		// console.log("onSearch", value);
	};

	onLoadData = (node: LegacyDataNode) => {
		return new Promise(async (resolve) => {
			if (node.dataType == "Tree") {
				const tree: ClassifierTree = node.dataRef;

				if (!tree.children) {
					const { field } = this.props,
						{ trees } = this.state;

					const children = await this._classifierGroupService.list(
						{ typeCode: field.props.typeCode, treeUid: tree.uid, parentUid: null });

					tree.children = children.rows;

					this.setState({ trees });
				}
			}
			else {
				const group: ClassifierGroup = node.dataRef;

				if (!group.children) {
					const { field } = this.props,
						{ groups } = this.state;

					const children = await this._classifierGroupService.list(
						{ typeCode: field.props.typeCode, treeUid: group.treeUid, parentUid: group.uid });

					group.children = children.rows;

					this.setState({ groups });
				}
			}

			resolve(null);
		});
	};

	buildTree(trees: ClassifierTree[], groups: ClassifierGroup[]): LegacyDataNode[] {
		if (trees) {
			return trees.map(tree => {

				const result: LegacyDataNode = {
					selectable: false,
					value: tree.uid.toString(),
					title: <span>{Icon.Folder} {tree.name}</span>,
					dataRef: tree,
					dataType: "Tree",
					props: undefined
				};

				if (tree.children) {
					result.children = this.buildTree(null, tree.children);
				}

				return result;
			});
		}
		else if (groups) {
			return groups.map(group => {

				const result: LegacyDataNode = {
					value: group.uid.toString(),
					title: <span>{Icon.File} {group.name} ({group.code})</span>,
					dataRef: group,
					dataType: "Group",
					props: undefined
				};

				if (group.children) {
					result.children = this.buildTree(null, group.children);
				}

				return result;
			});
		}
	}

	render = (): React.ReactNode => {
		const { value, field } = this.props,
			{ loading, expanded, trees, groups } = this.state;

		const treeData = this.buildTree(trees, groups);

		return (
			<Spin spinning={loading}>
				{/* For empty data show Select instead of TreeSelect.
					Empty TreeSelect fails with TypeError: Cannot read property 'scrollTo' of undefined */}
				{!loading && (!treeData || treeData.length == 0) && <Select
					placeholder={field.placeholder}
					value={value}
				/>}
				{!loading && treeData?.length > 0 && <TreeSelect
					onChange={this.handleChange}
					allowClear={!field.required}
					showSearch onSearch={this.onSearch}
					placeholder={field.placeholder}
					treeDefaultExpandedKeys={expanded.map(x => x.toString())}
					loadData={this.onLoadData}
					treeData={treeData}
					value={value}
				/>}
			</Spin>
		);
	};
}
