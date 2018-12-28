import { Injectable } from '@angular/core';
import { HttpService, RequestMethod } from './http.service';
import { Observable } from 'rxjs';
import { User } from '../models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = "/users";
  constructor(
    private httpService: HttpService
  ) { }

  get(uid: string): Observable<User> {
    return this.httpService.sendRequest(RequestMethod.Get, this.apiUrl, uid);
  }

  getOnlineUsers(): Observable<User[]> {
    return this.httpService.sendRequest(RequestMethod.Get, `${this.apiUrl}/online`);
  }

  getOnlineUsersByNameStartsWith(part: string): Observable<User[]> {
    return this.httpService.sendRequest(RequestMethod.Get, `${this.apiUrl}/online`, part);
  }

  add(user: User): Observable<User> {
    return this.httpService.sendRequest(RequestMethod.Post, this.apiUrl, undefined, user);
  }

  update(user: User): Observable<User> {
    return this.httpService.sendRequest(RequestMethod.Put, this.apiUrl, undefined, user);
  }
}
