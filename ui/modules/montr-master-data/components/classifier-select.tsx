import * as React from "react";
import { TreeSelect, Spin } from "antd";
import { IClassifierField, Guid } from "@montr-core/models";
import { ClassifierGroupService } from "../services";
import { IClassifierGroup } from "../models";
import { TreeNode } from "antd/lib/tree-select";

interface IProps {
	value?: string;
	field: IClassifierField;
	onChange?: (value: any) => void;
}

interface IState {
	loading: boolean;
	value: string;
	groups: IClassifierGroup[];
	treeData: TreeNode[];
}

// http://ant.design/components/form/?locale=en-US#components-form-demo-customized-form-controls
// https://github.com/ant-design/ant-design/blob/master/components/form/demo/customized-form-controls.md

export class ClassifierSelect extends React.Component<IProps, IState> {
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
			treeData: []
		};
	}

	buildTree = (groups: IClassifierGroup[]): TreeNode[] => {
		return groups && groups.map(x => {
			const result: TreeNode = {
				value: x.uid,
				title: x.name,
				// isLeaf: false
			};

			if (x.children) {
				result.children = this.buildTree(x.children);
			}

			return result;
		});
	}

	componentDidMount = async () => {
		const { field } = this.props,
			{ value } = this.state;

		// todo: load all groups to show selected value
		const groups = await this._classifierGroupService.list(
			new Guid("6465dd4c-8664-4433-ba6a-14effd40ebed"),
			{ typeCode: field.typeCode, treeCode: field.treeCode, focusUid: value });

		const treeData = this.buildTree(groups);

		this.setState({ loading: false, groups, treeData });
	}

	handleChange = (value: any, label: any, extra: any) => {
		this.triggerChange(value);
	}

	triggerChange = (changedValue: string) => {
		// Should provide an event to pass value to Form.
		const onChange = this.props.onChange;
		if (onChange) {
			onChange(changedValue);
		}
	}

	render = () => {
		const { value, field } = this.props,
			{ loading, treeData } = this.state;

		return (
			<Spin spinning={loading}>
				<TreeSelect
					onChange={this.handleChange}
					allowClear showSearch
					placeholder={field.placeholder}
					treeDefaultExpandAll
					treeData={treeData}
					value={value}
				/>
			</Spin>
		);
	}
}
