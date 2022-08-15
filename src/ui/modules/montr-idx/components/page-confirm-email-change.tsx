import { Page } from "@montr-core/components";
import { withNavigate, withParams } from "@montr-core/components/react-router-wrappers";
import { Patterns } from "@montr-core/module";
import { Button, Spin } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { NavigateFunction } from "react-router-dom";
import { Locale } from "../module";
import { AccountService } from "../services/account-service";

interface RouteProps {
	userId?: string;
	email?: string;
	code?: string;
}

interface Props {
	params: RouteProps;
	navigate: NavigateFunction;
}
interface State {
	loading: boolean;
}

class ConfirmEmailChange extends React.Component<Props, State> {

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
		const { userId, email, code } = this.getRouteProps();

		const result = await this.accountService.confirmEmailChange({ userId, email, code });

		if (result.success) {
			this.setState({ loading: false });
		}
	};

	handleContinue = async () => {
		this.props.navigate(Patterns.dashboard);
	};

	render = () => {
		const { loading } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.confirmEmailChange.title")}>
					<Spin spinning={loading}>

						{!loading && <p>Thank you for confirming your email change.</p>}

						<Button disabled={loading} onClick={this.handleContinue}>{t("button.continue")}</Button>
					</Spin>
				</Page>}
			</Translation>
		);
	};
}

export default withNavigate(withParams(ConfirmEmailChange));
