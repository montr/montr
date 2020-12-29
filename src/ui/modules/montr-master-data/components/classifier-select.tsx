import * as React from "react";
import { Spin, Select, Divider, Button } from "antd";
import { Guid } from "@montr-core/models";
import { ClassifierService } from "../services";
import { IClassifierField, ClassifierGroup, ClassifierTree, ClassifierType, Classifier } from "../models";
import { RouteBuilder } from "../module";
import { Link } from "react-router-dom";
import { Icon } from "@montr-core/components";
// import { debounce } from "lodash";

interface IProps {
	value?: string;
	field: IClassifierField;
	onChange?: (value: any) => void;
}

interface IState {
	loading: boolean;
	fetching: boolean;
	items?: Classifier[];

	value: string;
	type?: ClassifierType;
	trees?: ClassifierTree[];
	groups?: ClassifierGroup[];
	expanded: Guid[];
}

// http://ant.design/components/form/?locale=en-US#components-form-demo-customized-form-controls
// https://github.com/ant-design/ant-design/blob/master/components/form/demo/customized-form-controls.md
// todo: rewrite to functional component (see link above)
export class ClassifierSelect extends React.Component<IProps, IState> {

	static getDerivedStateFromProps(nextProps: any) {
		// Should be a controlled component.
		if ("value" in nextProps) {
			return nextProps.value ?? null;
		}
		return null;
	}

	private _classifierService = new ClassifierService();

	constructor(props: IProps) {
		super(props);

		// this.lastFetchId = 0;

		this.state = {
			loading: true,
			fetching: false,
			value: props.value,
			expanded: []
		};

		// this.onSearch = debounce(this.onSearch, 800);
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.value !== prevProps.value) {
			// await this.fetchData();
		}
	};

	componentWillUnmount = async () => {
		await this._classifierService.abort();
	};

	fetchData = async () => {
		const { field } = this.props,
			{ value } = this.state;

		const data = await this._classifierService.list({
			typeCode: field.props.typeCode, focusUid: value, pageSize: 1000
		});

		this.setState({ loading: false, items: data.rows });
	};

	handleChange = (value: any/* , label: any, extra: any */) => {
		const { onChange } = this.props;

		this.setState({
			value,
			// items: [], // ???
			fetching: false,
		});

		if (onChange) {
			onChange(value);
		}
	};

	onSearch = async (value: string) => {
		const { field } = this.props;

		this.setState({ items: [], fetching: true });

		const data = await this._classifierService.list({
			typeCode: field.props.typeCode, searchTerm: value
		});

		this.setState({ items: data.rows, fetching: false });
	};

	render() {
		const { value, field } = this.props,
			{ loading, fetching, items } = this.state;

		const options = items
			&& items.map(x => <Select.Option key={x.uid.toString()} value={x.uid.toString()}>{x.name}</Select.Option>);

		// https://github.com/ant-design/ant-design/issues/13448
		// https://codesandbox.io/s/oo6q47mnr9

		return (<Select
			value={value}
			loading={loading}
			showArrow={true}
			showSearch={true}
			autoClearSearchValue={false}
			onSearch={this.onSearch}
			notFoundContent={fetching ? <Spin size="small" /> : null}
			filterOption={false}
			placeholder={field.placeholder}
			allowClear={!field.required}
			onChange={this.handleChange}
			dropdownRender={menu => (
				<div>
					{menu}
					<Divider style={{ margin: "1px 0" }} />
					<div onMouseDown={e => e.preventDefault()}>
						<Link to={RouteBuilder.addClassifier(field.props.typeCode, null)}>
							<Button type="link" icon={Icon.Plus}>Добавить элемент</Button>
						</Link>
					</div >
				</div>
			)}>
			{options}
		</Select>);
	}
}
