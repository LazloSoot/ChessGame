import { Injectable } from "@angular/core";
import { HttpService, RequestMethod } from "./http.service";
import { Observable } from "rxjs";
import { User, Game, Page } from "../models";

@Injectable({
	providedIn: "root"
})
export class UserService {
	private apiUrl = "/users";
	constructor(private httpService: HttpService) {}

	getCurrentUser(): Observable<User> {
		return this.httpService.sendRequest(
			RequestMethod.Get,
			`${this.apiUrl}/current`
		);
	}

	get(id: number): Observable<User> {
		return this.httpService.sendRequest(
			RequestMethod.Get, 
			this.apiUrl, 
			id);
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

	getOnlineUsersByNameStartsWith(part: string, page?: Page): Promise<User[]> {
		return this.httpService
			.sendRequest(RequestMethod.Get, `${this.apiUrl}/online`, part, page)
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

	getUserGames(userId: number, page?: Page): Observable<Game[]> {
		return this.httpService.sendRequest(
			RequestMethod.Get, 
			`${this.apiUrl}/${userId}/games`, undefined, page);
	}

	add(user: User): Observable<User> {
		return this.httpService.sendRequest(
			RequestMethod.Post,
			this.apiUrl,
			undefined,
			undefined,
			user
		);
	}

	update(user: User): Observable<User> {
		return this.httpService.sendRequest(
			RequestMethod.Put,
			this.apiUrl,
			undefined,
			undefined,
			user
		);
	}
}
