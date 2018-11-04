import * as React from 'react';
import { List, Badge } from "antd";

import { EventTemplate } from '../api/EventTemplate';
import { API } from '../api/EventTemplates';

interface Props {
}

interface State {
  data: EventTemplate[];
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
        <h2>Торговые процедуры</h2>
        
        
      </div>
    );
  }
}
