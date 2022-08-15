import React from "react";
import { useNavigate, useParams } from "react-router-dom";

export const withNavigate = (Component: React.ElementType) => {
	const Wrapper = (props: any) => {
		return (
			<Component navigate={useNavigate()} {...props} />
		);
	};

	return Wrapper;
};

export const withParams = (Component: React.ElementType) => {
	const Wrapper = (props: any) => {
		return (
			<Component params={useParams()} {...props} />
		);
	};

	return Wrapper;
};
