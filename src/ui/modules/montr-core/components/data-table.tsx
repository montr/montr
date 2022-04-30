import LockTwoTone from "@ant-design/icons/lib/icons/LockTwoTone";
import { Divider, Table, Typography } from "antd";
import { ColumnType, SorterResult, SortOrder, TablePaginationConfig } from "antd/lib/table/interface";
import * as React from "react";
import { Link } from "react-router-dom";
import { Icon, StatusTag } from ".";
import { Constants } from "..";
import { DataColumn, DataResult, IIndexer, IMenu, Paging } from "../models";
import { DateHelper, Fetcher, MetadataService, NotificationService } from "../services";

interface Props<TModel> {
	rowKey?: string | ((record: TModel, index: number) => string);
	rowActions?: IMenu[];
	viewId?: string;
	columns?: DataColumn[],
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

interface DataTableColumnTemplate {
	name: string;
	template: (value: unknown, record: unknown, index: number) => React.ReactNode;
}

export abstract class DataTableTemplateRegistry {
	static Templates: DataTableColumnTemplate[] = [];

	static add(items: DataTableColumnTemplate[]): void {
		Array.prototype.push.apply(DataTableTemplateRegistry.Templates, items);
	}

	static getTemplate(name: string): (text: unknown, record: unknown, index: number) => React.ReactNode {
		return name && DataTableTemplateRegistry.Templates.find(x => x.name == name)?.template;
	}
}

export interface DataTableUpdateToken {
	date: Date;
	resetCurrentPage?: boolean;
	resetSelectedRows?: boolean;
}

export class DataTable<TModel extends IIndexer> extends React.Component<Props<TModel>, State<TModel>> {

	private readonly fetcher = new Fetcher();
	private readonly metadataService = new MetadataService();
	private readonly notificationService = new NotificationService();

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
		await this.updateMetadata();
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
		} else if (this.props.viewId !== prevProps.viewId
			|| this.props.columns !== prevProps.columns) {
			await this.updateMetadata();
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
			sorter.order == "ascend" ? "Ascending"
				: sorter.order == "descend" ? "Descending"
					: undefined;

		this.setState({ paging }, () => this.fetchData());
	};

	updateMetadata = async (): Promise<void> => {
		try {

			const { paging } = this.state;

			const metaColumns = await this.fetchMetadata();

			if (!metaColumns) return;

			const rcColumns = this.convertColumns(metaColumns);

			const defaultSortColumn =
				metaColumns.filter((col: DataColumn) => col.defaultSortOrder)[0];

			if (defaultSortColumn) {
				// fixme: why sorting columns stored in paging?
				// todo: fix sorting indicator on initial load
				paging.sortColumn = defaultSortColumn.key;
				paging.sortOrder = defaultSortColumn.defaultSortOrder;
			}

			this.setState({ columns: rcColumns, paging }, () => this.fetchData());

		} catch (error) {
			this.setState({ error });
			// todo: localize
			this.notificationService.error("Error loading metadata", error.message);
			throw error;
		}
	};

	fetchMetadata = async (): Promise<DataColumn[]> => {
		const { viewId, columns } = this.props;

		if (columns) return columns;

		if (viewId) {
			return await (await this.metadataService.load(viewId))?.columns;
		}

		return null;
	};

	convertColumns = (metaColumns: DataColumn[]): ColumnType<TModel>[] => {

		const { rowActions } = this.props;

		const rcColumns = metaColumns.map((item: DataColumn): ColumnType<TModel> => {

			const template =
				DataTableTemplateRegistry.getTemplate(item.template)
				?? DataTableTemplateRegistry.getTemplate(`type:${item.type}`)
				?? ((text: unknown): React.ReactNode => <>text</>);

			const render = (value: unknown, record: TModel, index: number): React.ReactNode => {
				const node = template(value, record, index);
				const url: string = item.urlProperty && record[item.urlProperty];
				return url ? <Link to={url}>{node}</Link> : node;
			};

			let defaultSortOrder: SortOrder | undefined = undefined;
			if (item.defaultSortOrder == "Ascending") defaultSortOrder = "ascend";
			else if (item.defaultSortOrder == "Descending") defaultSortOrder = "descend";

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

		if (rowActions?.length > 0) {
			rcColumns.push({
				key: "$action",
				title: "Action", // todo: localize
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

		return rcColumns;
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
			this.notificationService.error("Error loading data", error.message);
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
		const { rowKey = "id", skipPaging, viewId } = this.props,
			{ loading, columns, data, selectedRowKeys, paging, totalCount } = this.state;

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

		return (<>
			<Table size="small"
				rowKey={rowKey}
				columns={columns}
				dataSource={data}
				pagination={skipPaging ? false : pagination}
				loading={loading}
				onChange={this.handleTableChange}
				rowSelection={rowSelection}
			/* title={() =>
				<Toolbar clear size="small" float="right">
					<Button type="link" icon={Icon.Setting} />
				</Toolbar>
			} */
			/>

			<Typography.Text type="secondary" copyable>{viewId}</Typography.Text>
		</>);
	};
}

DataTableTemplateRegistry.add([
	{ name: "type:boolean", template: (value: unknown) => value ? Icon.Check : null },
	{ name: "type:date", template: (value: string | Date) => DateHelper.toLocaleDateString(value) },
	{ name: "type:time", template: (value: string | Date) => DateHelper.toLocaleTimeString(value) },
	{ name: "type:datetime", template: (value: string | Date) => DateHelper.toLocaleDateTimeString(value) },
	{ name: "code", template: (value: string) => <Typography.Text code>{value}</Typography.Text> },
	{ name: "status", template: (value: string) => <StatusTag statusCode={value} /> },

	{
		name: "metadata-field-name", template: (value: unknown, record: IIndexer) => {
			return <>{value} {record["system"] && <LockTwoTone twoToneColor="red" title="System" />}</>;
		}
	}
]);
