import { ErrorBoundary, Footer, Logo } from "@montr-core/components";
import { Layout } from "antd";
import * as React from "react";

interface Props {
	children: React.ReactNode;
}

export class AuthLayout extends React.Component<Props> {
	render = () => {
		return (
			<Layout className="public-layout auth-layout">
				<Layout.Content style={{ padding: "0 50px" }}>
					<ErrorBoundary>
						<div style={{ width: "480px", margin: "0 auto" }}>
							<Logo />
							{this.props.children}
						</div>
					</ErrorBoundary>
				</Layout.Content>
				<Layout.Footer>
					<ErrorBoundary>
						<Footer />
					</ErrorBoundary>
				</Layout.Footer>
			</Layout>
		);
	};
}
