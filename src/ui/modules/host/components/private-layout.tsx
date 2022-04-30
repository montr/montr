import { DataMenu, ErrorBoundary, Footer } from "@montr-core/components";
import { UserWithCompanyMenu } from "@montr-kompany/components";
import { Layout } from "antd";
import * as React from "react";

interface Props {
	children: React.ReactNode;
}

export class PrivateLayout extends React.Component<Props> {

	render = () => {

		const siderWidth = 220, theme = "light";

		return (
			<Layout hasSider className="private-layout bg-white">
				<Layout.Sider
					collapsible
					theme={theme}
					breakpoint="lg"
					width={siderWidth}
				/* onBreakpoint={broken => { console.log("Sider.onBreakpoint", broken); }}
				onCollapse={(collapsed, type) => { console.log("Sider.onCollapse", collapsed, type); }} */
				>
					<ErrorBoundary>
						{/* <Logo /> */}
						<DataMenu
							menuId="SideMenu"
							theme={theme}
							mode="inline"
							tail={
								<UserWithCompanyMenu />
							}
						/>
					</ErrorBoundary>
				</Layout.Sider>
				<Layout>
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
	};
}
