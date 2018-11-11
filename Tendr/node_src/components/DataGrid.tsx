import * as React from "react";

import { Link } from "react-router-dom";

import { Table } from "antd";
import { ColumnProps } from "antd/lib/table";

import { MetadataAPI, DataColumn, Indexer, Fetcher } from "../api";

interface DataGridProps {
	viewId: string
	loadUrl: string; // todo: add load func
}

interface DataGridState {
	columns: any[];
	data: Event[];
}

export class DataGrid<TModel extends Indexer> extends React.Component<DataGridProps, DataGridState> {

	constructor(props: DataGridProps) {
		super(props);
		this.state = { columns: [], data: [] };
	}

	componentDidMount() {
		MetadataAPI
			.load(this.props.viewId)
			.then((data) => {

				const columns = data.map((item: DataColumn): ColumnProps<TModel> => {

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

		Fetcher
			.post(this.props.loadUrl)
			.then((data) => {
				this.setState({ data });
			});
	}

	render() {
		return (
			<Table size="small" rowKey="id"
				columns={this.state.columns} dataSource={this.state.data}
				pagination={{ position: "both", pageSize: 10 }} />
		);
	}
}