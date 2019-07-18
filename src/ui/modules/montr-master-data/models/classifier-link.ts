import { IClassifier, IClassifierGroup, IClassifierTree } from ".";

export interface IClassifierLink {
	tree?: IClassifierTree;
	group?: IClassifierGroup;
	item?: IClassifier;
}
