import React from "react";
import { Alert } from "antd";

interface Props {
	children: React.ReactNode;
}

interface State {
	hasError: boolean;
	error?: Error;
	errorInfo?: React.ErrorInfo;
}

export class ErrorBoundary extends React.Component<Props, State> {

	constructor(props: Props) {
		super(props);

		this.state = {
			hasError: false
		};
	}

	static getDerivedStateFromError(_: Error): State {
		return { hasError: true };
	}

	componentDidCatch(error: Error, errorInfo: React.ErrorInfo): void {

		this.setState({
			error: error,
			errorInfo: errorInfo
		});

		// logErrorToService(error, errorInfo);
	}

	render = () => {

		if (this.state.hasError) {
			return (
				<Alert
					type="error"
					showIcon
					message={this.state.error?.toString()}
					description={
						<details>
							<p style={{ whiteSpace: "pre-wrap" }}>
								{this.state.errorInfo?.componentStack}
							</p>
						</details>
					}
				/>
			);
		}

		return this.props.children;
	};
}
