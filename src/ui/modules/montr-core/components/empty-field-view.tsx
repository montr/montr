import { css } from "@emotion/react";
// import { theme } from "antd";
import React from "react";

// const { token } = theme.useToken();

const style = css({
	color: '#aaa' // token.colorBgBase
});

export const EmptyFieldView = () => (
	<span css={style}>No data</span>
);
