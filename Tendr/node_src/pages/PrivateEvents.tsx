import * as React from 'react';
import { Table, Form, Select, Button, Divider } from "antd";

import { EventTemplate, API } from '../api/EventTemplates';
import { PageHeader } from '../components/PageHeader';

interface Props {
}

interface State {
  data: EventTemplate[];
}

const columns = [{
  title: 'Номер',
  dataIndex: 'name',
  width: 100,
}, {
  title: 'Дата',
  dataIndex: 'age',
  width: 150,
  sorter: (a, b) => a.age - b.age,
}, {
  title: 'Наименование',
  dataIndex: 'address',
}];

const data = [];
for (let i = 1; i < 10000; i++) {
  data.push({
    key: i,
    name: `T-00${i}`,
    age: 32 + i % 34,
    address: `London, Park Lane no. ${i}`,
  });
}

export class PrivateEvents extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { data: [] };
  }

  public componentDidMount() {
    API.fetchEventTemplates()
      .then((data) => {
        this.setState({ data });
      });
  }

  public render() {
    return (
      <div>
        <PageHeader>Торговые процедуры</PageHeader>

        <Form layout="inline">
          <Form.Item>
            <Select mode="multiple" placeholder="Выберите тип" style={{ minWidth: 200 }}>
              <Select.Option value="0">Запрос предложений</Select.Option>
              <Select.Option value="1">Предложение</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item>
            <Button type="primary">Искать</Button>
          </Form.Item>
        </Form>

        <br />

        <Table size="small" columns={columns} dataSource={data}
          pagination={{ pageSize: 10 }} />
      </div>
    );
  }
}
