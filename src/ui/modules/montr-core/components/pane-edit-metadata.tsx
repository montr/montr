import * as React from "react";
import { Alert, Button } from "antd";

export class PaneEditMetadata extends React.Component {

	render() {
		return (<>
			<Alert type="info"
				message="Настройка метаданных" />

			<Button>Добавить</Button>
		</>
		);
	}
}
