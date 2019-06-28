import * as React from "react";

declare const Sizes: ["small", "default", "large"];
declare const Floats: ["left", "right"];

interface IProps {
	size?: (typeof Sizes)[number];
	float?: (typeof Floats)[number];
}

export class Toolbar extends React.Component<IProps> {
	render = () => {

		const { size, float, children } = this.props;

		return (
			<div className={`toolbar toolbar-${size || "default"} toolbar-${float || "left"}`}>
				{children}
			</div>
		);
	}
}
