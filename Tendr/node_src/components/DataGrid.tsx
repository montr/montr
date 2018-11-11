import * as React from "react";

import { Link } from "react-router-dom";

import { Table } from "antd";
import { ColumnProps } from "antd/lib/table";

import { MetadataAPI, IDataColumn, IIndexer, Fetcher } from "../api";

interface DataGridProps {
	viewId: string
	loadUrl: string; // todo: add load func or data[]
}

interface DataGridState<TModel> {
	columns: any[];
	data: TModel[];
}

export class DataGrid<TModel extends IIndexer> extends React.Component<DataGridProps, DataGridState<TModel>> {

	constructor(props: DataGridProps) {
		super(props);
		this.state = { columns: [], data: [] };
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

	fetchData() {
		Fetcher
			.post(this.props.loadUrl)
			.then((data) => {
				this.setState({ data });
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
				pagination={{ position: "both", pageSize: 10 }} />
		);
	}
}