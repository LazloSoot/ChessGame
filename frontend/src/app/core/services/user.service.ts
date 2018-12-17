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

  add(user: User): Observable<User> {
    return this.httpService.sendRequest(RequestMethod.Post, this.apiUrl, undefined, user);
  }

  update(user: User): Observable<User> {
    return this.httpService.sendRequest(RequestMethod.Put, this.apiUrl, undefined, user);
  }
}
