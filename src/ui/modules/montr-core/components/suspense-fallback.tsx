import React from "react";
import { Spin } from "antd";

export const SuspenseFallback = () => (
	<Spin size="large" style={{ position: "fixed", top: "33%", left: "49%" }} />
);
