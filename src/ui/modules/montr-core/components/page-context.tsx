import React from "react";

export interface PageContextProps {
    isEditMode: boolean;
    setEditMode: (isEditMode: boolean) => void;
}

const defaultState: PageContextProps = {
    isEditMode: false,
    setEditMode: (isEditMode: boolean) => { return; }
};

export const PageContext = React.createContext<PageContextProps>(defaultState);

export function withPageContext<P extends PageContextProps>(Component: React.ComponentType<P>) {
    return (props: Pick<P, Exclude<keyof P, keyof PageContextProps>>) => (
        <PageContext.Consumer>
            {(ctx) => (
                <Component {...props} {...ctx as P} />
            )}
        </PageContext.Consumer>
    );
}

interface PageContextState {
    isEditMode: boolean;
}

export class PageContextProvider extends React.Component<unknown, PageContextState> {

    constructor(props: unknown) {
        super(props);

        this.state = {
            isEditMode: false
        };
    }

    setEditMode = (isEditMode: boolean): void => {
        this.setState({ isEditMode });
    };

    render = (): React.ReactNode => {
        const { isEditMode } = this.state;

        const context: PageContextProps = {
            isEditMode,
            setEditMode: this.setEditMode
        };

        return (
            <PageContext.Provider value={context}>
                {this.props.children}
            </PageContext.Provider>
        );
    };
}
