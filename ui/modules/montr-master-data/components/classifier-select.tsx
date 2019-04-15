import * as React from "react";
import { TreeSelect } from "antd";
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
	value: string;
	groups: IClassifierGroup[];
	treeData: TreeNode[];
}

// http://ant.design/components/form/?locale=en-US#components-form-demo-customized-form-controls
// https://github.com/ant-design/ant-design/blob/master/components/form/demo/customized-form-controls.md

export class ClassifierSelect extends React.Component<IProps, IState> {
	static getDerivedStateFromProps(nextProps: any) {

		console.log("getDerivedStateFromProps", nextProps);

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
			value: props.value,
			groups: [],
			treeData: []
		};
	}

	componentDidMount = async () => {
		const { field } = this.props;

		// todo: load all groups to show selected value
		const groups = await this._classifierGroupService.list(
			new Guid("6465dd4c-8664-4433-ba6a-14effd40ebed"), field.typeCode, field.treeCode, null);

		const treeData = groups.map(x => { return { value: x.uid, title: x.name } });

		this.setState({ groups, treeData });
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
			{ treeData } = this.state;

		return (
			<TreeSelect
				onChange={this.handleChange}
				allowClear showSearch
				placeholder={field.placeholder}
				treeData={treeData}
				value={value}
			/>
		);
	}
}
