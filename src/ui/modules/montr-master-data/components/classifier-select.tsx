import * as React from "react";
import { TreeSelect, Spin, Icon, Select } from "antd";
import { IClassifierField, Guid } from "@montr-core/models";
import { ClassifierGroupService, ClassifierTreeService, ClassifierTypeService, ClassifierService } from "../services";
import { IClassifierGroup, IClassifierTree, IClassifierType, IClassifier } from "../models";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { TreeNode } from "antd/lib/tree-select";

interface IProps extends CompanyContextProps {
	// mode: "Tree" | "Group";
	value?: string;
	field: IClassifierField;
	onChange?: (value: any) => void;
}

interface IState {
	loading: boolean;
	items?: IClassifier[];

	value: string;
	type?: IClassifierType;
	trees?: IClassifierTree[];
	groups?: IClassifierGroup[];
	expanded: Guid[];
}

// http://ant.design/components/form/?locale=en-US#components-form-demo-customized-form-controls
// https://github.com/ant-design/ant-design/blob/master/components/form/demo/customized-form-controls.md
// todo: rewrite to functional component (see link above)
class _ClassifierSelect extends React.Component<IProps, IState> {

	static getDerivedStateFromProps(nextProps: any) {
		// Should be a controlled component.
		if ("value" in nextProps) {
			return nextProps.value;
		}
		return null;
	}

	private _classifierTypeService = new ClassifierTypeService();
	private _classifierTreeService = new ClassifierTreeService();
	private _classifierGroupService = new ClassifierGroupService();
	private _classifierService = new ClassifierService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			// items: [],
			value: props.value,
			expanded: []
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.value !== prevProps.value) {
			// await this.fetchData();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
		await this._classifierTreeService.abort();
		await this._classifierGroupService.abort();
		await this._classifierService.abort();
	}

	fetchData = async () => {
		const { currentCompany, field } = this.props,
			{ value, expanded } = this.state;

		if (currentCompany) {

			const data = await this._classifierService.list(currentCompany.uid, { typeCode: field.typeCode });

			this.setState({ loading: false, items: data.rows });

			/* const type = await this._classifierTypeService.get(currentCompany.uid, { typeCode: field.typeCode });

			let trees: IClassifierTree[], groups: IClassifierGroup[];

			if (type.hierarchyType == "Groups") {
				if (field.treeCode || field.treeUid) {
					const result = await this._classifierGroupService.list(
						currentCompany.uid, { typeCode: field.typeCode, treeCode: field.treeCode, treeUid: field.treeUid, focusUid: value });

					groups = result.rows;

					await this.collectExpanded(groups, expanded);
				}
				else {
					const result = await this._classifierTreeService.list(currentCompany.uid, { typeCode: field.typeCode });

					trees = result.rows;
				}
			}
			else if (type.hierarchyType == "Items") {
				const result = await this._classifierGroupService.list(
					currentCompany.uid, { typeCode: field.typeCode, focusUid: value });

				groups = result.rows;

				await this.collectExpanded(groups, expanded);
			}

			this.setState({ loading: false, type, trees, groups, expanded }); */
		}
	}

	async collectExpanded(groups: IClassifierGroup[], expanded?: Guid[]) {
		groups && groups.forEach(group => {
			if (group.children) {

				expanded.push(group.uid);

				this.collectExpanded(group.children, expanded);
			}
		});
	}

	handleChange = (value: any/* , label: any, extra: any */) => {
		// Should provide an event to pass value to Form.
		const { onChange } = this.props;

		// console.log("handleChange", value);

		if (onChange) {
			onChange(value);
		}
	}

	onSearch = (value: string) => {
		// console.log("onSearch", value);
	}

	onLoadData = (node: any) => {
		return new Promise(async (resolve) => {

			if (node.props.dataType == "Tree") {
				const tree: IClassifierTree = node.props.dataRef;

				if (!tree.children) {
					const { currentCompany, field } = this.props,
						{ trees } = this.state;

					const children = await this._classifierGroupService.list(
						currentCompany.uid, { typeCode: field.typeCode, treeUid: tree.uid, parentUid: null });

					tree.children = children.rows;

					this.setState({ trees });
				}
			}
			else {
				const group: IClassifierGroup = node.props.dataRef;

				if (!group.children) {
					const { currentCompany, field } = this.props,
						{ groups } = this.state;

					const children = await this._classifierGroupService.list(
						currentCompany.uid, { typeCode: field.typeCode, treeUid: group.treeUid, parentUid: group.uid });

					group.children = children.rows;

					this.setState({ groups });
				}
			}

			resolve();
		});
	}

	buildTree(trees: IClassifierTree[], groups: IClassifierGroup[]): TreeNode[] {
		if (trees) {
			return trees.map(tree => {

				const result: TreeNode = {
					selectable: false,
					value: tree.uid,
					title: <span><Icon type="folder" /> {tree.name}</span>,
					dataRef: tree,
					dataType: "Tree"
				};

				if (tree.children) {
					result.children = this.buildTree(null, tree.children);
				}

				return result;
			});
		}
		else if (groups) {
			return groups && groups.map(group => {

				const result: TreeNode = {
					value: group.uid,
					title: <span><Icon type="file" /> {group.name} ({group.code})</span>,
					dataRef: group,
					dataType: "Group"
				};

				if (group.children) {
					result.children = this.buildTree(null, group.children);
				}

				return result;
			});
		}
	}

	render() {
		const { value, field } = this.props,
			{ loading, items, expanded, trees, groups } = this.state;

		const treeData = this.buildTree(trees, groups);

		return (
			<Select
				loading={loading}
				value={value}
				placeholder={field.placeholder}
				allowClear={!field.required}
				onChange={this.handleChange}
			/*

			showSearch onSearch={this.onSearch}

			treeDefaultExpandedKeys={expanded.map(x => x.toString())}
			loadData={this.onLoadData}
			treeData={treeData} */
			>
				{items && items.map(x => <Select.Option key={x.uid.toString()}>{x.name}</Select.Option>)}
			</Select>);
	}
}

export const ClassifierSelect = withCompanyContext(_ClassifierSelect);
