import * as React from "react";

import { Link } from "react-router-dom";

import { Table } from "antd";
import { ColumnProps, PaginationConfig, SorterResult } from "antd/lib/table";

import { MetadataAPI, IDataColumn, IIndexer, Fetcher, IDataResult } from "../api";

interface DataGridProps {
	viewId: string
	loadUrl: string; // todo: add load func or data[]
}

interface DataGridState<TModel> {
	columns: any[];
	data: TModel[];
	pagination: PaginationConfig,
	loading: boolean
}

export class DataGrid<TModel extends IIndexer> extends React.Component<DataGridProps, DataGridState<TModel>> {

	constructor(props: DataGridProps) {
		super(props);

		this.state = {
			columns: [],
			data: [],
			pagination: { position: "both", pageSize: 10 },
			loading: false,
		};
	}

	handleTableChange = (pagination: PaginationConfig, filters: Record<keyof TModel, string[]>, sorter: SorterResult<TModel>) => {

		const pager = { ...this.state.pagination };

		pager.current = pagination.current;

		this.setState({
			pagination: pager,
		});

		this.fetchData({
			pageSize: pagination.pageSize,
			pageNo: pagination.current,
			sortColumn: sorter.field,
			sortOrder: sorter.order == "ascend" ? "ascending" : "descending",
			// ...filters,
		});
	}

	fetchMetadata() {
		MetadataAPI
			.load(this.props.viewId)
			.then((data) => {

				const columns = data.map((item: IDataColumn): ColumnProps<TModel> => {

					var render: (text: any, record: TModel, index: number) => React.ReactNode;

					if (item.urlProperty) {
						render = (text: any, record: TModel, index: number): React.ReactNode => {
							const cellUrl: string = record[item.urlProperty];
							return (<Link to={cellUrl}>{text}</Link>);
						};
					}

					return {
						key: item.key,
						dataIndex: item.path || item.key,
						title: item.name,
						align: item.align,
						sorter: item.sortable,
						width: item.width,
						render: render
					};
				});

				this.setState({ columns });
			});
	}

	fetchData(params = {}) {

		this.setState({
			loading: true
		});

		Fetcher
			.post(this.props.loadUrl, {
				// results: 10,
				...params,
			})
			.then((data: IDataResult<TModel>) => {

				const pagination = { ...this.state.pagination };

				pagination.total = data.totalCount;

				this.setState({
					loading: false,
					pagination,
					data: data.rows
				});
			});
	}

	componentDidMount() {
		this.fetchMetadata();
		this.fetchData();
	}

	render() {
		return (
			<Table size="small" rowKey="id"
				columns={this.state.columns} dataSource={this.state.data}
				pagination={this.state.pagination}
				loading={this.state.loading}
				onChange={this.handleTableChange} />
		);
	}
}