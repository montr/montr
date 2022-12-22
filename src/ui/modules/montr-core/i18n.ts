import i18n from "i18next";
import i18nextLanguageDetector from "i18next-browser-languagedetector";
import i18nextBackend from "i18next-xhr-backend";
import { initReactI18next } from "react-i18next";

const defaultNS = "common";

i18n
	.use(i18nextBackend)
	.use(i18nextLanguageDetector)
	.use(initReactI18next)
	.init({
		// https://www.i18next.com/overview/configuration-options
		ns: defaultNS,
		defaultNS: defaultNS,
		fallbackNS: defaultNS,
		fallbackLng: "en",
		debug: false, // todo: use env var
		interpolation: {
			escapeValue: false,
		},
		react: {
			useSuspense: true,
		},
		// https://github.com/i18next/i18next-xhr-backend
		backend: {
			loadPath: "/api/locale/strings/{{lng}}/{{ns}}",
		},
		// https://github.com/i18next/i18next-browser-languageDetector
		detection: {
			order: ["cookie", "header"],
			lookupCookie: "lang",
			caches: ["cookie"]
		}
	});

export default i18n;
