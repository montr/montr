import * as React from 'react';
import { Table, Form, Select, Button } from "antd";

import { Event, EventAPI } from '../api';
import { PageHeader } from '../components/';

interface Props {
}

interface State {
  data: Event[];
}

const columns = [
  {
    title: 'Номер',
    dataIndex: 'number',
    width: 100
  },
  {
    title: 'Тип',
    dataIndex: 'eventType',
    width: 70
  },
  {
    title: 'Наименование',
    dataIndex: 'name'
  },
  {
    title: 'Описание',
    dataIndex: 'description'
  }
];

export class PrivateEvents extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { data: [] };
  }

  public componentDidMount() {
    EventAPI.fetchData()
      .then((data) => {
        this.setState({ data });
      });
  }

  public render() {
    return (
      <div>
        <div style={{ float: "right" }}>
          <Button icon="plus">Создать</Button>
        </div>

        <PageHeader>Торговые процедуры</PageHeader>

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

        <Table size="small" columns={columns} dataSource={this.state.data}
          pagination={{ pageSize: 10 }} />
      </div>
    );
  }
}
