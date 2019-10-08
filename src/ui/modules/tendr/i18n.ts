import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import i18nextBackend from "i18next-xhr-backend";
import i18nextLanguageDetector from "i18next-browser-languagedetector";

/* const resources = {
	en: {
		translation: {
			"confirm": {
				"title": "Confirm operation"
			},
			"publish": {
				"confirm": {
					"content": "Are you sure you want to publish the event?"
				}
			}
		}
	},
	ru: {
		translation: {
			"confirm": {
				"title": "Подтверждение операции"
			},
			"publish": {
				"confirm": {
					"content": "Вы действительно хотите опубликовать событие?"
				}
			}
		}
	}
}; */

i18n
	.use(i18nextBackend)
	.use(i18nextLanguageDetector)
	.use(initReactI18next)
	.init({
		// https://www.i18next.com/overview/configuration-options
		defaultNS: "common",
		lng: "en",
		whitelist: ["en", "ru"],
		fallbackLng: "en",
		debug: true,
		// https://www.i18next.com/overview/configuration-options#missing-keys
		saveMissing: false,
		interpolation: {
			escapeValue: false,
		},
		react: {
			bindI18n: "languageChanged",
			useSuspense: true,
		},
		// https://github.com/i18next/i18next-xhr-backend
		backend: {
			loadPath: "/assets/locales/{{lng}}/{{ns}}.json"
		},
		// https://github.com/i18next/i18next-browser-languageDetector
		detection: {
			// order: ["cookie", "navigator"]
		}
	});

export default i18n;
