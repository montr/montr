import React from "react";

interface IProps {
}

interface IState {
}

export class EmptyFieldView extends React.Component<IProps, IState> {
	render = () => {
		return (
			<span className="empty-field">No data</span>
		);
	};
}
