import * as React from "react";
import { Classifier, ClassifierType } from "../models";
import { FormEditClassifier } from ".";

interface Props {
	type: ClassifierType;
	data: Classifier;
	onDataChange?: (data: Classifier) => void;
}

export default class TabEditClassifier extends React.Component<Props> {

	render = (): React.ReactNode => {

		const { type, data, onDataChange } = this.props;

		return (<>
			{data && <FormEditClassifier
				type={type}
				uid={data?.uid}
				data={data}
				onSuccess={onDataChange}
			/>}
		</>);
	};
}
