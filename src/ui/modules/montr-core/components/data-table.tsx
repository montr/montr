import * as React from "react";
import { Link } from "react-router-dom";
import { Table, Tag, Divider } from "antd";
import { PaginationConfig } from "antd/lib/pagination";
import { SorterResult, SortOrder, ColumnType, TablePaginationConfig } from "antd/lib/table/interface";
import { Fetcher, NotificationService, MetadataService } from "../services";
import { IIndexer, IDataColumn, IDataResult, IMenu, IPaging } from "../models";
import { Constants } from "..";
import { Icon } from ".";

interface IProps<TModel> {
	rowKey?: string | ((record: TModel, index: number) => string);
	rowActions?: IMenu[];
	viewId: string;
	loadUrl: string; // todo: (?) add data[]
	// todo: add type for post params
	onLoadData?: (loadUrl: string, postParams: any) => Promise<IDataResult<TModel>>;
	onSelectionChange?: (selectedRowKeys: string[] | number[], selectedRows: TModel[]) => void;
	skipPaging?: boolean;
	updateToken?: DataTableUpdateToken;
}

interface IState<TModel> {
	loading: boolean;
	selectedRowKeys: string[] | number[];
	error?: any;
	columns: any[];
	data: TModel[];
	totalCount: number;
	paging: IPaging;
}

export class DataTableUpdateToken {
	date: Date;
	resetCurrentPage?: boolean;
	resetSelectedRows?: boolean;
}

export class DataTable<TModel extends IIndexer> extends React.Component<IProps<TModel>, IState<TModel>> {

	private _fetcher = new Fetcher();
	private _metadataService = new MetadataService();
	private _notification = new NotificationService();

	constructor(props: IProps<TModel>) {
		super(props);

		this.state = {
			loading: false,
			selectedRowKeys: [],
			columns: [],
			data: [],
			totalCount: 0,
			paging: {
				pageNo: 1,
				pageSize: Constants.defaultPageSize,
				skipPaging: props.skipPaging
			},
		};
	}

	componentDidMount = async () => {
		await this.fetchMetadata();
	};

	componentDidUpdate = async (prevProps: IProps<TModel>) => {
		if (this.props.updateToken !== prevProps.updateToken) {

			const { updateToken } = this.props,
				{ paging, selectedRowKeys } = this.state;

			if (updateToken?.resetCurrentPage !== false) {
				paging.pageNo = 1;
			}

			this.setState({
				paging,
				selectedRowKeys: updateToken?.resetSelectedRows ? [] : selectedRowKeys
			}, () => this.fetchData());
		}
	};

	componentWillUnmount = async () => {
		await this._fetcher.abort();
		await this._metadataService.abort();
	};

	handleTableChange = async (pagination: PaginationConfig,
		filters: Record<keyof TModel, string[]>, sorter: SorterResult<TModel>) => {

		const { paging } = this.state;

		paging.pageNo = pagination.current;
		paging.pageSize = pagination.pageSize;

		// todo: check other field types
		// todo: add support of multiple sort columns
		let sortColumn: string;
		if (sorter.field instanceof Array) sortColumn = sorter.field[0] as string;
		if (sorter.field instanceof String) sortColumn = sorter.field as string;

		paging.sortColumn = sortColumn;
		paging.sortOrder =
			sorter.order == "ascend" ? "ascending"
				: sorter.order == "descend" ? "descending"
					: null;

		this.setState({ paging }, () => this.fetchData());
	};

	fetchMetadata = async () => {
		const { viewId, rowActions } = this.props,
			{ paging } = this.state;

		const dataView = await this._metadataService.load(viewId);

		const columns = dataView.columns.map((item: IDataColumn): ColumnType<TModel> => {

			var render: (text: any, record: TModel, index: number) => React.ReactNode;

			if (item.urlProperty) {
				render = (text: any, record: TModel, index: number): React.ReactNode => {
					const cellUrl: string = record[item.urlProperty];
					return (cellUrl ? <Link to={cellUrl}>{text}</Link> : text);
				};
			}

			if (item.type == "boolean") {
				render = (text: any, record: TModel, index: number): React.ReactNode => {
					return text ? Icon.get("check") : null;
				};
			}

			// todo: remove
			if (item.key == "configCode") {
				render = (text: any, record: TModel, index: number): React.ReactNode => {
					return <Tag color="blue">{text}</Tag>;
				};
			}

			// todo: remove
			if (item.key == "statusCode") {
				render = (text: any, record: TModel, index: number): React.ReactNode => {
					return <Tag color="green">{text}</Tag>;
				};
			}

			let defaultSortOrder: SortOrder;
			if (item.defaultSortOrder == "ascending") defaultSortOrder = "ascend";
			else if (item.defaultSortOrder == "descending") defaultSortOrder = "descend";

			return {
				key: item.key,
				dataIndex: (item.path || item.key).split("."),
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

		if (rowActions && rowActions.length > 0) {
			columns.push({
				key: "$action",
				title: "Действие",
				width: 80,
				render: (text: any, record: TModel, index: number) => (
					<span>
						{rowActions.map((action, i) => {
							return (<React.Fragment key={`action-${i}`}>
								{i > 0 && <Divider type="vertical" />}

								{action.onClick &&
									<a onClick={() => action.onClick.call(this, record, index)}>{action.name}</a>
								}

								{(action.route) &&
									<Link to={(typeof action.route == "string") ? action.route as string : action.route(record, index)}>{action.name}</Link>
								}

							</React.Fragment>);
						})}
					</span>
				)
			});
		}

		const defaultSortColumn =
			dataView.columns.filter((col: IDataColumn) => col.defaultSortOrder)[0];

		paging.sortColumn = defaultSortColumn?.key;
		paging.sortOrder = defaultSortColumn?.defaultSortOrder;

		this.setState({ columns, paging }, () => this.fetchData());
	};

	fetchData = async () => {

		this.setState({ loading: true });

		const { loadUrl, onLoadData } = this.props,
			{ paging } = this.state;

		try {
			let postParams: any = { ...paging };

			const data: IDataResult<TModel> = onLoadData
				? await onLoadData(loadUrl, postParams)
				: await this._fetcher.post(loadUrl, postParams);

			if (data) {
				this.setState({
					loading: false,
					paging,
					// todo: save all data in state?
					totalCount: data.totalCount,
					data: data.rows
				});
			}
			else {
				this.setState({
					loading: false
				});
			}

		} catch (error) {
			this.setState({ error, loading: false });
			// todo: localize (?)
			this._notification.error("Ошибка загрузки данных.", error.message);
			throw error;
		}
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[], selectedRows: TModel[]) => {
		this.setState({ selectedRowKeys });

		if (this.props.onSelectionChange) {
			this.props.onSelectionChange(selectedRowKeys, selectedRows);
		}
	};

	render() {
		const { skipPaging } = this.props,
			{ selectedRowKeys, paging, totalCount } = this.state;

		const rowSelection = {
			columnWidth: 1,
			selectedRowKeys,
			onChange: this.onSelectionChange
		};

		// todo: localize
		const pagination: TablePaginationConfig = {
			showTotal: (total: number, range: [number, number]) => {
				return (<>
					{selectedRowKeys.length > 0 && (<span style={{ marginRight: "1em" }}>Выбрано: <strong>{selectedRowKeys.length}</strong></span>)}
					{(total != 0) && (<span>Записи <strong>{range[0]}</strong> &mdash; <strong>{range[1]}</strong> из <strong>{total}</strong></span>)}
				</>);
			},
			current: paging.pageNo,
			pageSize: paging.pageSize,
			pageSizeOptions: ["10", "50", "100", "500"],
			showSizeChanger: true,
			total: totalCount
		};

		return (
			<Table size="small"
				rowKey={this.props.rowKey || "id"}
				columns={this.state.columns}
				dataSource={this.state.data}
				pagination={skipPaging ? false : pagination}
				loading={this.state.loading}
				onChange={this.handleTableChange}
				rowSelection={rowSelection}
			/>
		);
	}
}
