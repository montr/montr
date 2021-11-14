import React from "react";

export interface PageContextProps {
    isEditMode: boolean;
    setEditMode: (isEditMode: boolean) => void;
    addPageEventListener: (listener: PageEventListener) => void;
    removePageEventListener: (listener: PageEventListener) => void;
    onPageSubmit: () => void;
    onPageCancel: () => void;
}

export interface PageEventListener {
    onPageSubmit: () => void;
    onPageCancel: () => void;
}

const defaultState: PageContextProps = {
    isEditMode: false,
    setEditMode: (isEditMode: boolean) => { return; },
    addPageEventListener: (listener: PageEventListener) => { return; },
    removePageEventListener: (listener: PageEventListener) => { return; },
    onPageSubmit: () => { return; },
    onPageCancel: () => { return; }
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

    private readonly listeners: PageEventListener[] = [];

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
            setEditMode: this.setEditMode,
            addPageEventListener: (listener: PageEventListener) => {
                this.listeners.push(listener);
            },
            removePageEventListener: (listener: PageEventListener) => {
                const index = this.listeners.findIndex(x => x == listener);
                if (index >= 0) this.listeners.splice(index, 1);
            },
            onPageSubmit: () => {
                this.listeners.forEach(x => x.onPageSubmit());
            },
            onPageCancel: () => {
                this.listeners.forEach(x => x.onPageCancel());
            }
        };

        return (
            <PageContext.Provider value={context}>
                {this.props.children}
            </PageContext.Provider>
        );
    };
}
