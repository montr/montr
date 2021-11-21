import React from "react";

export interface PageContextProps extends PageEventListener {
    isEditMode: boolean;
    isDirty: boolean;
    setEditMode: (isEditMode: boolean) => void;
    setDirty: (isDirty: boolean) => void;
    addPageEventListener: (listener: PageEventListener) => void;
    removePageEventListener: (listener: PageEventListener) => void;
}

export interface PageEventListener {
    onPageSubmit: () => Promise<void>;
    onPageSubmitted: () => Promise<void>;
    onPageCancel: () => Promise<void>;
}

const defaultState: PageContextProps = {
    isEditMode: false,
    isDirty: false,
    setEditMode: (isEditMode: boolean) => { return; },
    setDirty: (isDirty: boolean) => { return; },
    addPageEventListener: (listener: PageEventListener) => { return; },
    removePageEventListener: (listener: PageEventListener) => { return; },
    onPageSubmit: async () => { return; },
    onPageSubmitted: async () => { return; },
    onPageCancel: async () => { return; }
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
    isDirty: boolean;
}

export class PageContextProvider extends React.Component<unknown, PageContextState> {

    private readonly listeners: PageEventListener[] = [];

    constructor(props: unknown) {
        super(props);

        this.state = {
            isEditMode: false,
            isDirty: false
        };
    }

    setEditMode = (isEditMode: boolean): void => {
        this.setState({ isEditMode, isDirty: false });
    };

    setDirty = (isDirty: boolean): void => {
        this.setState({ isDirty });
    };

    addPageEventListener = (listener: PageEventListener) => {
        this.listeners.push(listener);
    };

    removePageEventListener = (listener: PageEventListener) => {
        const index = this.listeners.findIndex(x => x == listener);
        if (index >= 0) this.listeners.splice(index, 1);
    };

    onPageSubmit = async () => {
        await Promise.all(this.listeners.map(x => x.onPageSubmit()));
    };

    onPageSubmitted = async () => {
        await Promise.all(this.listeners.map(x => x.onPageSubmitted()));
    };
    onPageCancel = async () => {
        await Promise.all(this.listeners.map(x => x.onPageCancel()));
    };

    render = (): React.ReactNode => {
        const { isEditMode, isDirty } = this.state;

        const context: PageContextProps = {
            isEditMode,
            isDirty,
            setEditMode: this.setEditMode,
            setDirty: this.setDirty,
            addPageEventListener: this.addPageEventListener,
            removePageEventListener: this.removePageEventListener,
            onPageSubmit: this.onPageSubmit,
            onPageSubmitted: this.onPageSubmitted,
            onPageCancel: this.onPageCancel
        };

        return (
            <PageContext.Provider value={context}>
                {this.props.children}
            </PageContext.Provider>
        );
    };
}
