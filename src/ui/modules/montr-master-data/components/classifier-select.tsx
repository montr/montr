import * as React from "react";
import { Spin, Icon, Select, Divider, Button } from "antd";
import { IClassifierField, Guid } from "@montr-core/models";
import { ClassifierService } from "../services";
import { IClassifierGroup, IClassifierTree, IClassifierType, IClassifier } from "../models";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components";
import { RouteBuilder } from "../module";
import { Link } from "react-router-dom";
// import { debounce } from "lodash";

interface IProps extends CompanyContextProps {
	value?: string;
	field: IClassifierField;
	onChange?: (value: any) => void;
}

interface IState {
	loading: boolean;
	fetching: boolean;
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
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.value !== prevProps.value) {
			// await this.fetchData();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierService.abort();
	}

	fetchData = async () => {
		const { field } = this.props,
			{ value } = this.state;

		const data = await this._classifierService.list({
			typeCode: field.typeCode, focusUid: value, pageSize: 1000
		});

		this.setState({ loading: false, items: data.rows });
	}

	handleChange = (value: any/* , label: any, extra: any */) => {
		// Should provide an event to pass value to Form.
		const { onChange } = this.props;

		this.setState({
			value,
			// items: [], // ???
			fetching: false,
		});

		if (onChange) {
			onChange(value);
		}
	}

	onSearch = async (value: string) => {
		const { field } = this.props;

		this.setState({ items: [], fetching: true });

		const data = await this._classifierService.list({
			typeCode: field.typeCode, searchTerm: value
		});

		this.setState({ items: data.rows, fetching: false });
	}

	render() {
		const { value, field } = this.props,
			{ loading, fetching, items } = this.state;

		const options = items
			&& items.map(x => <Select.Option key={x.uid.toString()}>{x.name}</Select.Option>);

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
						<Link to={RouteBuilder.addClassifier(field.typeCode, null)}>
							<Button type="link"><Icon type="plus" /> Добавить элемент</Button>
						</Link>
					</div >
				</div>
			)}
		>
			{options}
		</Select>);
	}
}

export const ClassifierSelect = withCompanyContext(_ClassifierSelect);
