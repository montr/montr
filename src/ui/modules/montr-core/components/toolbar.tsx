import * as React from "react";

declare const Sizes: ["small", "default", "large"];
declare const Floats: ["left", "right", "bottom"];

interface Props {
	size?: (typeof Sizes)[number];
	float?: (typeof Floats)[number];
	clear?: boolean;
}

export class Toolbar extends React.Component<Props> {
	render = (): React.ReactNode => {

		const { size = "default", float = "left", clear, children } = this.props;

		return (<>
			<div className={`toolbar toolbar-${size} toolbar-${float}`}>
				{children}
			</div>

			{clear && <div style={{ clear: "both" }}></div>}
		</>);
	};
}
