import { IClassifier, IClassifierGroup } from ".";

export interface IClassifierLink {
	group?: IClassifierGroup;
	item?: IClassifier;
}
