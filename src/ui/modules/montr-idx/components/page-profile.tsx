import React from "react";
import { Translation } from "react-i18next";
import { Page, Toolbar, DataBreadcrumb, DataMenu, RouteList } from "@montr-core/components";
import { ProfileRoutes } from "../module";

interface Props {
}

interface State {
	mode: "inline" | "horizontal";
	selectedKey: string;
}

export default class Profile extends React.Component<Props, State> {

	private _main: HTMLDivElement | undefined = undefined;

	constructor(props: Props) {
		super(props);

		this.state = {
			mode: "inline",
			selectedKey: "profile"
		};
	}

	componentDidMount = async () => {
		// todo: fix lot of state changes and network operations, move to menu?
		/* window.addEventListener("resize", this.resize);
		this.resize(); */
	};

	componentWillUnmount = async () => {
		// window.removeEventListener("resize", this.resize);
	};

	resize = () => {
		if (!this._main) return;

		requestAnimationFrame(() => {
			if (!this._main) return;

			let mode: "inline" | "horizontal" = "inline";

			const { offsetWidth } = this._main;

			if (offsetWidth < 641 && offsetWidth > 400) {
				mode = "horizontal";
			}

			if (window.innerWidth < 768 && offsetWidth > 400) {
				mode = "horizontal";
			}

			if (this.state.mode != mode) this.setState({ mode });
		});
	};

	render = () => {
		const { mode } = this.state;

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
