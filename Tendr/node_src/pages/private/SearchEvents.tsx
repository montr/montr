import * as React from 'react';
import { Link } from "react-router-dom";
import { Table, Form, Select, Button } from "antd";

import { DataColumn, MetadataAPI, Event, EventAPI, DataColumnAlign } from '../../api';
import { Page } from '../../components';

interface Props {
}

interface State {
  columns: any[];
  data: Event[];
}

export class SearchEvents extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { columns: [], data: [] };
  }

  public componentDidMount() {
    MetadataAPI.fetchData("PrivateEventSearch/Grid")
      .then((data) => {

        function convertAlignToString(value: DataColumnAlign): string {
          switch (value) {
            case DataColumnAlign.Left: return "left";
            case DataColumnAlign.Right: return "right";
            case DataColumnAlign.Center: return "center";
            default: return null;
          }
        }

        const columns = data.map((item: DataColumn): any => {
          return {
            key: item.key,
            dataIndex: item.path || item.key,
            title: item.name,
            align: convertAlignToString(item.align),
            sorter: item.sortable,
            width: item.width,
          };
        });
        this.setState({ columns });
      });

    EventAPI.fetchData()
      .then((data) => {
        this.setState({ data });
      });
  }

  public render() {
    return (
      <Page title="Торговые процедуры">

        <div style={{ float: "right" }}>
            <Link to="/events/new"><Button icon="plus">Создать</Button></Link>
        </div>

        <Form layout="inline">
          <Form.Item>
            <Select mode="multiple" placeholder="Выберите тип" style={{ minWidth: 200 }}>
              <Select.Option value="0">Запрос предложений</Select.Option>
              <Select.Option value="1">Предложение</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item>
            <Button type="primary" icon="search">Искать</Button>
          </Form.Item>
        </Form>

        <br />

        <Table size="small" rowKey="id"
          columns={this.state.columns} dataSource={this.state.data}
          pagination={{ pageSize: 10 }} />
      </Page>
    );
  }
}
