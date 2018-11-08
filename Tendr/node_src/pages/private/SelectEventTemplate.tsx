import * as React from "react";
import { Button, List, Badge, Tag } from "antd";

import { EventTemplate, EventTemplateAPI } from "../../api";
import { Page } from "../../components";

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
      <Page title="Выберите шаблон процедуры">

        <div style={{ width: "50%" }}>

          <List size="small" bordered dataSource={this.state.data} itemLayout="horizontal"
            renderItem={(item: EventTemplate) => (
              <List.Item
                actions={[
                  <Button onClick={() => console.log(item.id) }>Выбрать</Button>
                ]}>
                <List.Item.Meta
                  title={item.name}
                  description={item.description}
                />
              </List.Item>
            )}
          />
        </div>

      </Page>
    );
  }
}
