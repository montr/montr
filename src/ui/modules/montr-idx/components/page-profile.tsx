import { DataBreadcrumb, DataMenu, Page, RouteList, Toolbar } from "@montr-core/components";
import React from "react";
import { Translation } from "react-i18next";
import { Locale, ProfileRoutes } from "../module";

interface State {
	mode: "inline" | "horizontal";
	selectedKey: string;
}

export default class PageProfile extends React.Component<unknown, State> {

	private main: HTMLDivElement | undefined = undefined;

	constructor(props: unknown) {
		super(props);

		this.state = {
			mode: "inline",
			selectedKey: "profile"
		};
	}

	componentDidMount = async (): Promise<void> => {
		// todo: fix lot of state changes and network operations, move to menu?
		/* window.addEventListener("resize", this.resize);
		this.resize(); */
	};

	componentWillUnmount = async (): Promise<void> => {
		// window.removeEventListener("resize", this.resize);
	};

	resize = (): void => {
		if (!this.main) return;

		requestAnimationFrame(() => {
			if (!this.main) return;

			let mode: "inline" | "horizontal" = "inline";

			const { offsetWidth } = this.main;

			if (offsetWidth < 641 && offsetWidth > 400) {
				mode = "horizontal";
			}

			if (window.innerWidth < 768 && offsetWidth > 400) {
				mode = "horizontal";
			}

			if (this.state.mode != mode) this.setState({ mode });
		});
	};

	render = (): React.ReactNode => {
		const { mode } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page
					title={<>
						<Toolbar float="right">
						</Toolbar>

						<DataBreadcrumb items={[]} />
					</>}>

					{/* todo: good names & create components */}
					<div className="grid-content">
						<div className="page-with-menu" ref={ref => { if (ref) { this.main = ref; } }}>
							<div className="menu">
								<DataMenu menuId="ProfileMenu" mode={mode} />
							</div>
							<div className="content">
								<RouteList routes={ProfileRoutes} />
							</div>
						</div>
					</div>
				</Page>}
			</Translation>
		);
	};
}
