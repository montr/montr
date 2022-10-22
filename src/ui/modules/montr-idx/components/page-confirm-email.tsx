import { Page } from "@montr-core/components";
import { withParams } from "@montr-core/components/react-router-wrappers";
import { Patterns } from "@montr-core/module";
import { Button, Spin } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { Navigate } from "react-router-dom";
import { Locale } from "../module";
import { AccountService } from "../services/account-service";

interface RouteProps {
	userId?: string;
	code?: string;
}

interface Props {
	params: RouteProps;
}

interface State {
	loading: boolean;
	navigateTo?: string;
}

class ConfirmEmail extends React.Component<Props, State> {

	private readonly accountService = new AccountService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	getRouteProps = (): RouteProps => {
		return this.props.params;
	};

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this.accountService.abort();
	};

	fetchData = async () => {
		const { userId, code } = this.getRouteProps();

		const result = await this.accountService.confirmEmail({ userId, code });

		if (result.success) {
			this.setState({ loading: false });
		}
	};

	handleContinue = async () => {
		this.setState({ navigateTo: Patterns.dashboard });
	};

	render = () => {
		const { loading, navigateTo } = this.state;

		if (navigateTo) {
			return <Navigate to={navigateTo} />;
		}

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.confirmEmail.title") as string}>
					<Spin spinning={loading}>

						{!loading && <p>Thank you for confirming your email.</p>}

						<Button disabled={loading} onClick={this.handleContinue}>{t("button.continue") as string}</Button>
					</Spin>
				</Page>}
			</Translation>
		);
	};
}

export default withParams(ConfirmEmail);
