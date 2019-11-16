import React from "react";
import { Button, Menu } from "antd";
import { Translation } from "react-i18next";
import { Page, Toolbar, DataBreadcrumb } from "@montr-core/components";
import { PaneEditProfile } from "../components/pane-edit-profile";

interface IProps {
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
			selectedKey: "base"
		};
	}

	componentDidMount = async () => {
		window.addEventListener("resize", this.resize);
		this.resize();
	};

	handleChangePassword = () => {
		/* const { t } = this.props;

		Modal.confirm({
			title: t("confirm.title"),
			content: t("cancel.confirm.content"),
			onOk: () => {
				this._eventService
					.cancel(this.props.match.params.uid)
					.then((result: IApiResult) => {
						message.success(t("operation.success"));
						this.fetchData();
					});
			}
		}); */
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

	renderChildren = () => {
		const { selectedKey } = this.state;

		switch (selectedKey) {
			case "base":
				return <PaneEditProfile />;
			default:
				break;
		}

		return null;
	};

	render = () => {
		const { mode, selectedKey } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page
					title={<>
						<Toolbar float="right">
							<Button onClick={this.handleChangePassword}>{t("button.changePassword")}</Button>
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
									<Menu.Item key="base">Profile</Menu.Item>
									<Menu.Item key="2">Security</Menu.Item>
								</Menu>
							</div>
							<div className="content">
								{this.renderChildren()}
							</div>
						</div>
					</div>
				</Page>}
			</Translation>
		);
	};
}
