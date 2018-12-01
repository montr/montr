import * as React from "react";

import { Link } from "react-router-dom";

import { Table } from "antd";
import { ColumnProps, PaginationConfig, SorterResult, SortOrder } from "antd/lib/table";

import { MetadataAPI, IDataColumn, IIndexer, Fetcher, IDataResult } from "../api";

interface DataTableProps {
	viewId: string
	loadUrl: string; // todo: add load func or data[]
}

interface DataTableState<TModel> {
	columns: any[];
	data: TModel[];
	pagination: PaginationConfig,
	loading: boolean
}

export class DataTable<TModel extends IIndexer> extends React.Component<DataTableProps, DataTableState<TModel>> {

	constructor(props: DataTableProps) {
		super(props);

		this.state = {
			columns: [],
			data: [],
			pagination: { position: "both", pageSize: 10, showSizeChanger: true, pageSizeOptions: ["10", "50", "100"] },
			loading: false,
		};
	}

	handleTableChange = (pagination: PaginationConfig, filters: Record<keyof TModel, string[]>, sorter: SorterResult<TModel>) => {

		const pager = { ...this.state.pagination };

		pager.current = pagination.current;
		pager.pageSize = pagination.pageSize;

		this.setState({
			pagination: pager,
		});

		this.fetchData({
			pageSize: pagination.pageSize,
			pageNo: pagination.current,
			sortColumn: sorter.field,
			sortOrder: sorter.order == "ascend"
				? "ascending" : sorter.order == "descend" ? "descending" : null,
			// ...filters,
		});
	}

	fetchMetadata() {
		MetadataAPI
			.load(this.props.viewId)
			.then((dataView) => {

				const columns = dataView.columns.map((item: IDataColumn): ColumnProps<TModel> => {

					var render: (text: any, record: TModel, index: number) => React.ReactNode;

					if (item.urlProperty) {
						render = (text: any, record: TModel, index: number): React.ReactNode => {
							const cellUrl: string = record[item.urlProperty];
							return (<Link to={cellUrl}>{text}</Link>);
						};
					}

					var defaultSortOrder: SortOrder;
					if (item.defaultSortOrder == "ascending") defaultSortOrder = "ascend";
					else if (item.defaultSortOrder == "descending") defaultSortOrder = "descend";

					return {
						key: item.key,
						dataIndex: item.path || item.key,
						title: item.name,
						align: item.align,
						sorter: item.sortable,
						defaultSortOrder: defaultSortOrder,
						// wtf: not enought for antd to set defaultSortOrder,
						// see getSortStateFromColumns() in https://github.com/ant-design/ant-design/blob/master/components/table/Table.tsx
						// sortOrder: defaultSortOrder, 
						width: item.width,
						render: render
					};
				});

				this.setState({ columns });

				const defaultSortColumn =
					dataView.columns.filter((col: IDataColumn) => col.defaultSortOrder)[0];

				this.fetchData({
					sortColumn: defaultSortColumn && defaultSortColumn.key,
					sortOrder: defaultSortColumn && defaultSortColumn.defaultSortOrder,
				})
			});
	}

	fetchData(params = {}) {

		// console.log(params);

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
		// this.fetchData();
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