import * as React from "react";
import { Layout } from "antd";
import { DataMenu, ErrorBoundary, Footer } from "@montr-core/components";
import { UserWithCompanyMenu } from "@montr-kompany/components";

export class PrivateLayout extends React.Component {

	render = () => {

		const siderWidth = 220;

		return (
			<Layout hasSider className="private-layout bg-white">
				<Layout.Sider theme="light" collapsible={false} width={siderWidth}
					style={{ overflow: 'auto', height: "100vh", position: 'fixed', left: 0 }}>
					<ErrorBoundary>
						{/* <div className="logo" /> */}

						<DataMenu
							menuId="SideMenu"
							theme="light"
							mode="inline"
							tail={
								<UserWithCompanyMenu />
							}
						/>
					</ErrorBoundary>
				</Layout.Sider>
				<Layout style={{ marginLeft: siderWidth }} className="bg-white">
					<Layout.Content className="bg-white">
						<ErrorBoundary>

							{this.props.children}

						</ErrorBoundary>
					</Layout.Content>
					<Layout.Footer className="bg-white">

						<ErrorBoundary>
							<Footer />
						</ErrorBoundary>

					</Layout.Footer>
				</Layout>
			</Layout>
		);
	}
}
