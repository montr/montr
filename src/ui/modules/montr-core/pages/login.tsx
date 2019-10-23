import * as React from "react";
import { Page } from "../components";

export default class Login extends React.Component {
	render() {
		return (
			<Page title="Log in">

				<p>Use a local account to log in.</p>

				<p>Use another service to log in.</p>

			</Page>
		);
	}
}
