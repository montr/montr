import * as React from "react";

declare const Sizes: ["small", "default", "large"];
declare const Floats: ["left", "right", "bottom"];

interface IProps {
	size?: (typeof Sizes)[number];
	float?: (typeof Floats)[number];
	clear?: boolean;
}

export class Toolbar extends React.Component<IProps> {
	render = () => {

		const { size, float, clear, children } = this.props;

		return (<>
			<div className={`toolbar toolbar-${size || "default"} toolbar-${float || "left"}`}>
				{children}
			</div>

			{clear && <div style={{ clear: "both" }}></div>}
		</>);
	};
}
