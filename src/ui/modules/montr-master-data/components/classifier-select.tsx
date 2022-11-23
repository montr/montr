import { Icon } from "@montr-core/components";
import { Guid } from "@montr-core/models";
import { Button, Divider, Select, Spin } from "antd";
import * as React from "react";
import { Link } from "react-router-dom";
import { Classifier, ClassifierGroup, ClassifierTree, ClassifierType, IClassifierField } from "../models";
import { RouteBuilder } from "../module";
import { ClassifierService } from "../services";
// import { debounce } from "lodash";

interface Props {
	value?: string;
	field: IClassifierField;
	onChange?: (value: any) => void;
}

interface State {
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
export class ClassifierSelect extends React.Component<Props, State> {

	static getDerivedStateFromProps(nextProps: any): void {
		// Should be a controlled component.
		if ("value" in nextProps) {
			return nextProps.value ?? null;
		}
		return null;
	}

	private readonly classifierService = new ClassifierService();

	constructor(props: Props) {
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

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.value !== prevProps.value) {
			// await this.fetchData();
		}
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.classifierService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { field } = this.props,
			{ value } = this.state;

		const data = await this.classifierService.list({
			typeCode: field.props.typeCode, focusUid: value, pageSize: 1000
		});

		this.setState({ loading: false, items: data?.rows });
	};

	handleChange = (value: any/* , label: any, extra: any */): void => {
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

	onSearch = async (value: string): Promise<void> => {
		const { field } = this.props;

		this.setState({ items: [], fetching: true });

		const data = await this.classifierService.list({
			typeCode: field.props.typeCode, searchTerm: value
		});

		this.setState({ items: data.rows, fetching: false });
	};

	render = (): React.ReactNode => {
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
			// allowClear={!field.required}
			onChange={this.handleChange}
			dropdownRender={menu => (
				<div>
					{menu}
					{field.props.typeCode && <>
						<Divider style={{ margin: "1px 0" }} />
						<div onMouseDown={e => e.preventDefault()}>
							<Link to={RouteBuilder.addClassifier(field.props.typeCode, null)}>
								<Button type="link" icon={Icon.Plus}>Добавить элемент</Button>
							</Link>
						</div>
					</>}
				</div>
			)}>
			{options}
		</Select>);
	};
}
