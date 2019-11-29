import React from "react";
import { Menu } from "antd";
import { Translation } from "react-i18next";
import { Page, Toolbar, DataBreadcrumb } from "@montr-core/components";
import { Route, RouteComponentProps, Switch } from "react-router";
import { Link } from "react-router-dom";
import { Patterns } from "@montr-idx/module";

interface IRouteProps {
	tabKey?: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
	mode: "inline" | "horizontal";
	selectedKey: string;
}

export default class Profile extends React.Component<IProps, IState> {

	private _main: HTMLDivElement | undefined = undefined;

	constructor(props: IProps) {
		super(props);

		this.state = {
			mode: "inline",
			selectedKey: "profile"
		};
	}

	componentDidMount = async () => {
		window.addEventListener("resize", this.resize);
		this.resize();
	};

	resize = () => {
		if (!this._main) return;

		requestAnimationFrame(() => {
			if (!this._main) return;

			let mode: "inline" | "horizontal" = "inline";

			const { offsetWidth } = this._main;

			if (this._main.offsetWidth < 641 && offsetWidth > 400) {
				mode = "horizontal";
			}

			if (window.innerWidth < 768 && offsetWidth > 400) {
				mode = "horizontal";
			}

			this.setState({ mode });
		});
	};

	render = () => {
		const { url, path } = this.props.match;
		const { mode, selectedKey } = this.state;

		return (
			<Translation ns="idx">
				{() => <Page
					title={<>
						<Toolbar float="right">
						</Toolbar>

						<DataBreadcrumb items={[]} />
					</>}>

					{/* todo: good names & create components */}
					<div className="grid-content">
						<div className="page-with-menu" ref={ref => { if (ref) { this._main = ref; } }}>
							<div className="menu">
								<Menu
									mode={mode}
									defaultSelectedKeys={[selectedKey]}
									onSelect={({ key }) => this.setState({ selectedKey: key })}>
									<Menu.Item key="profile"><Link to={url}>Profile</Link></Menu.Item>
									<Menu.Item key="security"><Link to={`${url}/security`}>Security</Link></Menu.Item>
									<Menu.Item key="external-logins"><Link to={`${url}/external-logins`}>External Logins</Link></Menu.Item>
									<Menu.Item key="notifications"><Link to={`${url}/notifications`}>Notifications</Link></Menu.Item>
									<Menu.Item key="history"><Link to={`${url}/history`}>History</Link></Menu.Item>
								</Menu>
							</div>
							<div className="content">
								<Switch>
									<Route path={`${path}`} exact component={React.lazy(() => import("../components/pane-edit-profile"))} />
									<Route path={`${path}/security`} component={React.lazy(() => import("../components/pane-security"))} />
									<Route path={`${path}/external-logins`} component={React.lazy(() => import("../components/pane-external-logins"))} />
									<Route path={Patterns.linkLogin} component={React.lazy(() => import("./link-login"))} />
									<Route>
										<h1>404</h1>
									</Route>
								</Switch>
							</div>
						</div>
					</div>
				</Page>}
			</Translation>
		);
	};
}
