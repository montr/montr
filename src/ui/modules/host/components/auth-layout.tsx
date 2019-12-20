import * as React from "react";
import { Layout } from "antd";
import { ErrorBoundary, Footer, Logo } from "@montr-core/components";

export class AuthLayout extends React.Component {
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
