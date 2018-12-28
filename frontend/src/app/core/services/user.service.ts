import { Injectable } from "@angular/core";
import { HttpService, RequestMethod } from "./http.service";
import { Observable } from "rxjs";
import { User } from "../models";

@Injectable({
	providedIn: "root"
})
export class UserService {
	private apiUrl = "/users";
	constructor(private httpService: HttpService) {}

	get(uid: string): Observable<User> {
		return this.httpService.sendRequest(
			RequestMethod.Get,
			this.apiUrl,
			uid
		);
	}

	getOnlineUsers(): Promise<User[]> {
		return this.httpService
			.sendRequest(RequestMethod.Get, `${this.apiUrl}/online`)
			.toPromise()
			.then(
				users => {
					if (users) {
						let usersResult: User[] = [];
						for (let prop in users) {
							usersResult.push(new User(prop, users[prop]));
						}
						return usersResult;
					}
				},
				error => {
					return error;
				}
			);
	}

	getOnlineUsersByNameStartsWith(part: string): Promise<User[]> {
		return this.httpService
			.sendRequest(RequestMethod.Get, `${this.apiUrl}/online`, part)
			.toPromise()
			.then(
				users => {
					if (users) {
						let usersResult: User[] = [];
						for (let prop in users) {
							usersResult.push(new User(prop, users[prop]));
						}
						return usersResult;
					}
				},
				error => {
					return error;
				}
			);
	}

	add(user: User): Observable<User> {
		return this.httpService.sendRequest(
			RequestMethod.Post,
			this.apiUrl,
			undefined,
			user
		);
	}

	update(user: User): Observable<User> {
		return this.httpService.sendRequest(
			RequestMethod.Put,
			this.apiUrl,
			undefined,
			user
		);
	}
}
