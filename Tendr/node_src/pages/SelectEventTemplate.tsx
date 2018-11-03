import * as React from 'react';
import { List, Badge } from "antd";

import { EventTemplate } from '../api/EventTemplate';
import { API } from '../api/EventTemplates';

interface Props {
}

interface State {
  data: EventTemplate[];
}

export class SelectEventTemplate extends React.Component<Props, State> {
  constructor(props : Props) {
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
      <div style={{ width: "50%" }}>
        <List size="small" bordered dataSource={this.state.data} itemLayout="horizontal"
          // pagination={{ pageSize: 10 }}
          renderItem={(item:EventTemplate) => (
            <List.Item
              actions={[ <Badge status="success" text={item.eventType.toString()} /> ]}>
              <List.Item.Meta
                title={<a href="#">{item.name}</a>}
                description={item.description}
              />
            </List.Item>
          )}
        />
      </div>
    );
  }
}
