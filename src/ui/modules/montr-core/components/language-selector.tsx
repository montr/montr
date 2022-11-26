import { css } from "@emotion/react";
import { Dropdown, theme } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { IIndexer } from "../models/indexer";
import { Icon } from "./icon";

export const LanguageSelector = () => {

	const { token } = theme.useToken();

	const style = css({
		color: token.colorTextDisabled,
		float: "right",
		paddingRight: 8,
		minWidth: 100
	});

	// todo: load from server
	const langs: IIndexer = {
		"en": { title: "English" },
		"ru": { title: "русский" },
	};

	return (
		<Translation>
			{(t, { i18n }) => (
				<Dropdown
					className="language-selector"
					css={style}
					menu={{
						items: Object.keys(langs).map(lng => ({
							key: lng, label: langs[lng].title
						})),
						onClick: (item) => i18n.changeLanguage(item.key)
					}}>
					<a className="ant-dropdown-link">
						{Icon.Global} {langs[i18n.language].title} {Icon.Down}
					</a>
				</Dropdown>
			)}
		</Translation>
	);
};
