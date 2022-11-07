import { ErrorBoundary } from "@montr-core/components/error-boundary";
import { Footer } from "@montr-core/components/footer";
import { Logo } from "@montr-core/components/logo";
import { Layout } from "antd";
import * as React from "react";
import { Outlet } from "react-router-dom";

export default class AuthLayout extends React.Component {
	render = () => {
		return (
			<Layout className="public-layout auth-layout">
				<Layout.Content style={{ padding: "0 50px" }}>
					<ErrorBoundary>
						<div style={{ width: "480px", margin: "0 auto" }}>
							<Logo />
							<Outlet />
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
