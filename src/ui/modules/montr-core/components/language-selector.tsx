import { Dropdown } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { Icon } from ".";
import { IIndexer } from "../models/indexer";

export const LanguageSelector = () => {

	// todo: load from server
	const langs: IIndexer = {
		"en": { title: "English" },
		"ru": { title: "русский" },
	};

	return (
		<Translation>
			{(t, { i18n }) => (
				<Dropdown
					trigger={["click"]}
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
