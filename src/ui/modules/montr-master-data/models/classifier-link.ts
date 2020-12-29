import { Classifier, ClassifierGroup, ClassifierTree } from ".";

export interface ClassifierLink {
	tree?: ClassifierTree;
	group?: ClassifierGroup;
	item?: Classifier;
}
