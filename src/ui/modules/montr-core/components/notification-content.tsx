import * as React from "react";

interface Props {
	messages: string[];
}

export class NotificationContent extends React.Component<Props> {

	public static build(messages: string[]): React.ReactNode {
		return <NotificationContent messages={messages} />;
	}

	render(): React.ReactNode {
		const { messages = [] } = this.props;

		return <ul>
			{messages.map((message, i) => <li key={`msg_${i}`}>{message}</li>)}
		</ul>;
	}
}
