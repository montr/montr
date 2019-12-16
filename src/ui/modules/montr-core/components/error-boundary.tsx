import React, { ErrorInfo } from "react";
import { Alert } from "antd";

interface Props {
}

interface State {
	error?: Error;
	errorInfo?: ErrorInfo;
}

export class ErrorBoundary extends React.Component<Props, State> {

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentDidCatch(error: Error, errorInfo: ErrorInfo): void {

		this.setState({
			error: error,
			errorInfo: errorInfo
		});

		// logErrorToService(error, errorInfo);
	}

	render = () => {

		if (this.state.error) {
			return (
				<Alert
					type="error"
					showIcon
					message="Error"
					description={
						<details>
							<p style={{ whiteSpace: "pre-wrap" }}>
								{this.state.error && this.state.error.toString()}
								{this.state.errorInfo && this.state.errorInfo.componentStack}
							</p>
						</details>
					}
				/>
			);
		}

		return this.props.children;
	};
}
