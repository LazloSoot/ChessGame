import { Injectable } from "@angular/core";
import { HttpService, RequestMethod } from "./http.service";
import { Observable } from "rxjs";
import { User, Game, Page, PagedResult } from "../models";

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

	getOnlineUsers(page: PagedResult<User>): Observable<PagedResult<User>> {
		return this.httpService
			.sendRequest(RequestMethod.Get, this.apiUrl, undefined, page);
	}

	getUsersByNameStartsWith(part: string, isOnline: boolean, page?: Page): Observable<PagedResult<User>> {
		let namedParams = {
			'part' : (part) ? part : '',
			'isOnline' : isOnline,
			'pageIndex' : (page) ? page.pageIndex : 0,
			'pageSize' : (page) ? page.pageSize : -1
		};
		return this.httpService
			.sendRequest(RequestMethod.Get, this.apiUrl, undefined, namedParams);
	}

	getUserGames(userId: number, page?: Page): Observable<PagedResult<Game>> {
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
