import * as React from "react";
import { Tree, TreeSelect, Spin } from "antd";
import { IClassifierField, Guid } from "@montr-core/models";
import { ClassifierGroupService } from "../services";
import { IClassifierGroup } from "../models";
// import { TreeNode } from "antd/lib/tree-select";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { TreeNode } from "antd/lib/tree-select";

interface IProps extends CompanyContextProps {
	value?: string;
	field: IClassifierField;
	onChange?: (value: any) => void;
}

interface IState {
	loading: boolean;
	value: string;
	groups: IClassifierGroup[];
	expanded: Guid[];
	// treeData: TreeNode[];
}

// http://ant.design/components/form/?locale=en-US#components-form-demo-customized-form-controls
// https://github.com/ant-design/ant-design/blob/master/components/form/demo/customized-form-controls.md

class _ClassifierSelect extends React.Component<IProps, IState> {
	static getDerivedStateFromProps(nextProps: any) {
		// Should be a controlled component.
		if ('value' in nextProps) {
			return {
				...(nextProps.value || {}),
			};
		}
		return null;
	}

	private _classifierGroupService = new ClassifierGroupService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			value: props.value,
			groups: [],
			expanded: [],
			// treeData: []
		};
	}

	buildTree(groups: IClassifierGroup[], expanded?: Guid[]): TreeNode[] {
		return groups && groups.map(group => {

			const result: TreeNode = {
				value: group.uid,
				title: `${group.code}. ${group.name}`,
				dataRef: group
			};

			if (group.children) {

				if (expanded) {
					expanded.push(group.uid);
				}

				result.children = this.buildTree(group.children, expanded);
			}

			return result;
		});
	}

	async componentDidMount() {
		const { currentCompany, field } = this.props,
			{ value } = this.state;

		if (currentCompany) {
			const groups = await this._classifierGroupService.list(
				currentCompany.uid, { typeCode: field.typeCode, /* treeCode: field.treeCode, */ focusUid: value });

			const expanded: Guid[] = [];
			// todo: only to detect expanded nodes
			/* const treeData = */ this.buildTree(groups.rows, expanded);

			this.setState({ loading: false, groups: groups.rows, expanded /* , treeData */ });
		}
	}

	handleChange = (value: any, label: any, extra: any) => {
		// Should provide an event to pass value to Form.
		const { onChange } = this.props;

		if (onChange) {
			onChange(value);
		}
	}

	onLoadData = (node: any) => {
		return new Promise(async (resolve) => {
			const group: IClassifierGroup = node.props.dataRef;

			if (group.children) {
				resolve();
				return;
			}

			const { currentCompany, field } = this.props,
				{ groups } = this.state;

			const children = await this._classifierGroupService.list(
				currentCompany.uid, { typeCode: field.typeCode, /* treeCode: field.treeCode, */ parentUid: group.uid });

			group.children = children.rows;

			this.setState({ groups });

			resolve();
		});
	}

	render() {
		const { value, field } = this.props,
			{ loading, expanded, groups } = this.state;

		const treeData = this.buildTree(groups);

		return (
			<Spin spinning={loading}>
				<TreeSelect
					onChange={this.handleChange}
					allowClear showSearch
					placeholder={field.placeholder}
					treeDefaultExpandedKeys={expanded.map(x => x.toString())}
					loadData={this.onLoadData}
					treeData={treeData}
					/* treeIcon */
					value={value}
				/>
			</Spin>
		);
	}
}

export const ClassifierSelect = withCompanyContext(_ClassifierSelect);
