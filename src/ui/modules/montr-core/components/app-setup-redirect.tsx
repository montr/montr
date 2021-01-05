import { useHistory } from "react-router";
import { Constants } from "..";
import { Patterns } from "../module";
import { AppState } from "../models";

interface Props {
    children?: JSX.Element;
}

export function AppSetupRedirect({ children }: Props): JSX.Element {

    const appState = Constants.appState,
        redirectTo = Patterns.setup,
        history = useHistory();

    if (appState == AppState.None && history.location.pathname != redirectTo) {

        // history.push(redirectTo);
    }

    return children;

};
