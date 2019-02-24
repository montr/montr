import { Guid } from "@montr-core/models";

export interface IClassifier {
	uid?: Guid;
	configCode?: string;
	statusCode?: string;
	code?: string;
	name?: string;
	url?: string;
}
