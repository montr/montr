import { css } from "@emotion/react";
import { theme } from "antd";
import React from "react";

export const EmptyFieldView = () => {
	const { token } = theme.useToken();

	const style = css({
		color: token.colorTextDisabled
	});

	return (
		<span css={style}>No data</span>
	);
};
