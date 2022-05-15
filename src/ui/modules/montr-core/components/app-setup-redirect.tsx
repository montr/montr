import { useLocation, useNavigate } from "react-router";
import { Constants } from "..";
import { AppState } from "../models";
import { Patterns } from "../module";

interface Props {
	children: JSX.Element;
}

export function AppSetupRedirect({ children }: Props): JSX.Element {

	const appState = Constants.appState,
		redirectTo = Patterns.setup,
		location = useLocation();

	if (appState == AppState.None && location.pathname != redirectTo) {
		const navigate = useNavigate();

		navigate(redirectTo);
	}

	return children;
}
