import * as React from "react";
import { Link } from "react-router-dom";
import { Table, Tag } from "antd";
import { ColumnProps, PaginationConfig, SorterResult, SortOrder } from "antd/lib/table";
import { IIndexer } from "@montr-core/models";
import { Fetcher, NotificationService, MetadataService } from "@montr-core/services";
import { IDataColumn, IDataResult } from "../models";

interface DataTableProps {
	rowKey?: string
	viewId: string
	loadUrl: string; // todo: (?) add load func or data[]
	postParams?: any;
}

interface DataTableState<TModel> {
	loading: boolean;
	error?: any;
	columns: any[];
	data: TModel[];
	pagination: PaginationConfig,
}

export class DataTable<TModel extends IIndexer> extends React.Component<DataTableProps, DataTableState<TModel>> {

	private _fetcher = new Fetcher();
	private _metadataService = new MetadataService();
	private _notification = new NotificationService();

	constructor(props: DataTableProps) {
		super(props);

		this.state = {
			loading: false,
			columns: [],
			data: [],
			pagination: {
				position: "bottom",
				pageSize: 50,
				pageSizeOptions: ["50", "100", "500"],
				showSizeChanger: true,
			},
		};
	}

	componentDidMount = async () => {
		await this.fetchMetadata();
	}

	componentDidUpdate = async (prevProps: DataTableProps) => {
		if (this.props.postParams !== prevProps.postParams) {
			await this.fetchMetadata();
		}
	}

	componentWillUnmount = async () => {
		await this._fetcher.abort();
		await this._metadataService.abort();
	}

	private handleTableChange = async (pagination: PaginationConfig,
		filters: Record<keyof TModel, string[]>, sorter: SorterResult<TModel>) => {

		const pager = { ...this.state.pagination };

		pager.current = pagination.current;
		pager.pageSize = pagination.pageSize;

		this.setState({
			pagination: pager,
		});

		await this.fetchData({
			pageSize: pagination.pageSize,
			pageNo: pagination.current,
			sortColumn: sorter.field,
			sortOrder: sorter.order == "ascend"
				? "ascending" : sorter.order == "descend" ? "descending" : null,
			// ...filters,
		});
	}

	private fetchMetadata = async () => {
		const dataView = await this._metadataService.load(this.props.viewId);

		const columns = dataView.columns.map((item: IDataColumn): ColumnProps<TModel> => {

			var render: (text: any, record: TModel, index: number) => React.ReactNode;

			if (item.urlProperty) {
				render = (text: any, record: TModel, index: number): React.ReactNode => {
					const cellUrl: string = record[item.urlProperty];
					return (cellUrl ? <Link to={cellUrl}>{text}</Link> : text);
				};
			}

			if (item.key == "configCode") {
				render = (text: any, record: TModel, index: number): React.ReactNode => {
					return <Tag color="blue">{text}</Tag>;
				};
			}

			if (item.key == "statusCode") {
				render = (text: any, record: TModel, index: number): React.ReactNode => {
					return <Tag color="green">{text}</Tag>;
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

		await this.fetchData({
			sortColumn: defaultSortColumn && defaultSortColumn.key,
			sortOrder: defaultSortColumn && defaultSortColumn.defaultSortOrder,
		})
	}

	private fetchData = async (params = {}) => {

		this.setState({ loading: true });

		try {
			let postParams = {
				pageSize: 50,
				...params,
			};

			if (this.props.postParams) {
				postParams = Object.assign(postParams, this.props.postParams);
			}

			const data: IDataResult<TModel> = await this._fetcher.post(this.props.loadUrl, postParams);

			const pagination = { ...this.state.pagination };

			pagination.total = data.totalCount;

			this.setState({
				loading: false,
				pagination,
				data: data.rows
			});
		} catch (error) {
			this.setState({ error, loading: false });
			this._notification.error({
				message: "Ошибка загрузки данных."
			});
			throw error;
		}
	}

	render = () => {
		const rowSelection = {
			columnWidth: 1
		};

		return <Table size="middle"
			rowKey={this.props.rowKey || "id"}
			columns={this.state.columns}
			dataSource={this.state.data}
			pagination={this.state.pagination}
			loading={this.state.loading}
			onChange={this.handleTableChange}
			rowSelection={rowSelection}
		/>
	}
}
