import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'

@Injectable()
export class AuthenticationService {
    constructor(private http: Http) { }

	login(username: string, password: string) {
		const headers = new Headers();
		headers.append('Content-Type', 'application/x-www-form-urlencoded');
		let options = new RequestOptions({ headers: headers });
		var content = "grant_type=" + "password" + "&username=" + username + "&password=" + password;
		return this.http.post('/token', content, options)
			.map((response: Response) => {
				let user = response.json();
				if (user && user.access_token) {
					localStorage.setItem('currentUser', JSON.stringify(user));
				}

				return user;
			});
	}

	ssologin(token: string, url: string, state: string) {
		return this.http.post(url + "?state=" + state + "&token=" + token, {})//JSON.stringify({ username: username }))
			.map((response: Response) => {

			});
	}

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
    }
}