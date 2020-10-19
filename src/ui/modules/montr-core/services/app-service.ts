import axios from "axios";
import { AppOptions } from "../models";

export class AppService {

	options = (): Promise<AppOptions> => {
		return axios
			.post("/api/app/options", {})
			.then(response => {
				console.log(response.data);
				return (response.data as AppOptions);
			});
	};

}
