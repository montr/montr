import { css } from "@emotion/react";
import * as React from "react";

declare const Sizes: ["small", "default", "large"];
declare const Floats: ["left", "right"];

interface Props {
	size?: (typeof Sizes)[number];
	float?: (typeof Floats)[number];
	clear?: boolean;
	children: React.ReactNode;
}

export const Toolbar = ({ size = "default", float = "left", clear, children }: Props) => {

	const style = css({
		float: float,
		marginBottom: 8,

		"&.toolbar-left": {
			".ant-btn, .ant-btn-group, .ant-radio-group, .ant-select": {
				marginRight: 6
			},
			".ant-btn-group .ant-btn": {
				marginRight: 0
			}
		},

		"&.toolbar-right": {
			".ant-btn, .ant-btn-group, .ant-radio-group, .ant-select": {
				marginLeft: 6
			}
		}
	});

	return (<>
		<div className={`toolbar toolbar-${size} toolbar-${float}`} css={style}>
			{children}
		</div>

		{clear && <div style={{ clear: "both" }}></div>}
	</>);
};
