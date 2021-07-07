import * as React from "react";
import { Link } from "react-router-dom";
import { Table, Tag, Divider } from "antd";
import { SorterResult, SortOrder, ColumnType, TablePaginationConfig } from "antd/lib/table/interface";
import { Fetcher, NotificationService, MetadataService, DateHelper } from "../services";
import { IIndexer, DataColumn, DataResult, IMenu, Paging } from "../models";
import { Constants } from "..";
import { Icon, StatusTag } from ".";

interface Props<TModel> {
	rowKey?: string | ((record: TModel, index: number) => string);
	rowActions?: IMenu[];
	viewId: string;
	loadUrl: string; // todo: (?) add data[]
	// todo: add type for post params
	onLoadData?: (loadUrl: string, postParams: unknown) => Promise<DataResult<TModel> | undefined>;
	onSelectionChange?: (selectedRowKeys: string[] | number[], selectedRows: TModel[]) => void;
	skipPaging?: boolean;
	updateToken?: DataTableUpdateToken;
}

interface State<TModel> {
	loading: boolean;
	selectedRowKeys: string[] | number[];
	error?: unknown;
	columns: ColumnType<TModel>[];
	data: TModel[];
	totalCount: number;
	paging: Paging;
}

export interface DataTableUpdateToken {
	date: Date;
	resetCurrentPage?: boolean;
	resetSelectedRows?: boolean;
}

export class DataTable<TModel extends IIndexer> extends React.Component<Props<TModel>, State<TModel>> {

	fetcher = new Fetcher();
	metadataService = new MetadataService();
	notificationService = new NotificationService();

	constructor(props: Props<TModel>) {
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

	componentDidMount = async (): Promise<void> => {
		await this.fetchMetadata();
	};

	componentDidUpdate = async (prevProps: Props<TModel>): Promise<void> => {
		if (this.props.updateToken !== prevProps.updateToken) {

			const { updateToken } = this.props,
				{ paging, selectedRowKeys } = this.state;

			if (updateToken?.resetCurrentPage === true) {
				paging.pageNo = 1;
			}

			this.setState({
				paging,
				selectedRowKeys: updateToken?.resetSelectedRows ? [] : selectedRowKeys
			}, () => this.fetchData());
		}
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.fetcher.abort();
		await this.metadataService.abort();
	};

	handleTableChange = async (pagination: TablePaginationConfig,
		filters: Record<keyof TModel, string[]>, sorter: SorterResult<TModel>): Promise<void> => {

		const { paging } = this.state;

		paging.pageNo = pagination.current;
		paging.pageSize = pagination.pageSize;

		// todo: check other field types
		// todo: add support of multiple sort columns
		let sortColumn: string | undefined = undefined;
		if (sorter.field instanceof Array) sortColumn = sorter.field[0] as string;
		if (sorter.field instanceof String) sortColumn = sorter.field as string;

		paging.sortColumn = sortColumn;
		paging.sortOrder =
			sorter.order == "ascend" ? "ascending"
				: sorter.order == "descend" ? "descending"
					: undefined;

		this.setState({ paging }, () => this.fetchData());
	};

	fetchMetadata = async (): Promise<void> => {
		const { viewId, rowActions } = this.props,
			{ paging } = this.state;

		const dataView = await this.metadataService.load(viewId);

		if (!dataView.columns) throw new Error("Metadata columns is empty");

		const columns = dataView.columns?.map((item: DataColumn): ColumnType<TModel> => {

			let render;

			if (item.urlProperty) {
				render = (text: unknown, record: TModel): React.ReactNode => {
					const url: string = record[item.urlProperty];
					return (url ? <Link to={url}>{text}</Link> : text);
				};
			}

			if (item.type == "boolean") {
				render = (text: unknown): React.ReactNode => {
					return text ? Icon.get("check") : null;
				};
			}

			if (item.type == "date") {
				render = (text: string | Date): React.ReactNode => {
					return DateHelper.toLocaleDateString(text);
				};
			}

			if (item.type == "time") {
				render = (text: string | Date): React.ReactNode => {
					return DateHelper.toLocaleTimeString(text);
				};
			}

			if (item.type == "datetime") {
				render = (text: string | Date): React.ReactNode => {
					return DateHelper.toLocaleDateTimeString(text);
				};
			}

			// todo: add support of custom renderers
			if (item.key == "configCode") {
				render = (text: unknown): React.ReactNode => {
					return <Tag color="blue">{text}</Tag>;
				};
			}

			// todo: add support of custom renderers
			if (item.key == "statusCode") {
				render = (text: string): React.ReactNode => {
					return <StatusTag statusCode={text} />;
				};
			}

			let defaultSortOrder: SortOrder | undefined = undefined;
			if (item.defaultSortOrder == "ascending") defaultSortOrder = "ascend";
			else if (item.defaultSortOrder == "descending") defaultSortOrder = "descend";

			return {
				key: item.key,
				dataIndex: (item.path ?? item.key).split("."),
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
				width: 40,
				render: (text: unknown, record: TModel, index: number) => (
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
			dataView.columns?.filter((col: DataColumn) => col.defaultSortOrder)[0];

		paging.sortColumn = defaultSortColumn?.key;
		paging.sortOrder = defaultSortColumn?.defaultSortOrder;

		this.setState({ columns, paging }, () => this.fetchData());
	};

	fetchData = async (): Promise<void> => {

		this.setState({ loading: true });

		const { loadUrl, onLoadData } = this.props,
			{ paging } = this.state;

		try {
			const postParams: unknown = { ...paging };

			const data: DataResult<TModel> = onLoadData
				? await onLoadData(loadUrl, postParams)
				: await this.fetcher.post(loadUrl, postParams);

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
			this.notificationService.error("Ошибка загрузки данных.", error.message);
			throw error;
		}
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[], selectedRows: TModel[]): Promise<void> => {
		this.setState({ selectedRowKeys });

		if (this.props.onSelectionChange) {
			this.props.onSelectionChange(selectedRowKeys, selectedRows);
		}
	};

	render = (): React.ReactNode => {
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
			/* title={() =>
				<Toolbar clear size="small" float="right">
					<Button type="link" icon={Icon.Setting} />
				</Toolbar>
			} */
			/>
		);
	};
}
