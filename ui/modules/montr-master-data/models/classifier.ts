import { Guid } from "@montr-core/models";

export interface IClassifier {
	uid?: Guid;
	configCode?: string;
	statusCode?: string;
	companyUid?: Guid,
	code?: string;
	name?: string;
	url?: string;
}
