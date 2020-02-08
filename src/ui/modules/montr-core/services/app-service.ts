import axios from "axios";
import { IAppOptions } from "../models";

export class AppService {

	options = (): Promise<IAppOptions> => {
		return axios
			.post("/api/app/options", {})
			.then(response => {
				console.log(response.data);
				return (response.data as IAppOptions);
			});
	};

}
