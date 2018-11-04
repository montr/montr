import * as React from 'react';
import { List, Badge, Tag } from "antd";

import { EventTemplate, EventTemplateAPI } from '../api/';
import { PageHeader } from '../components/';

interface Props {
}

interface State {
  data: EventTemplate[];
}

export class SelectEventTemplate extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { data: [] };
  }

  public componentDidMount() {
    EventTemplateAPI.fetchData()
      .then((data) => {
        this.setState({ data });
      });
  }

  public render() {
    return (
      <div>
        <PageHeader>Выберите шаблон процедуры</PageHeader>
        <div style={{ width: "50%" }}>

          <List size="small" bordered dataSource={this.state.data} itemLayout="horizontal"
            // pagination={{ pageSize: 10 }}
            renderItem={(item: EventTemplate) => (
              <List.Item
                actions={[
                  <Badge status="success" text={item.eventType.toString()} />,
                  <Tag>{item.eventType.toString()}</Tag>
                ]}>
                <List.Item.Meta
                  title={<a href="#">{item.name}</a>}
                  description={item.description}
                />
              </List.Item>
            )}
          />
        </div>
      </div>
    );
  }
}
