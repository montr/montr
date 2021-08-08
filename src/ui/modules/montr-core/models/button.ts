export interface Button {
    key?: string;
    name?: string;
    icon?: string;
    type?: "default" | "primary";
    action?: string;
    component?: string;
    props?: Record<string, unknown>;
}
