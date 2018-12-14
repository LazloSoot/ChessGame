import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _token: string;

  public token() {
    get: return this._token;
  }
  constructor() { }

  signUp() {

  }

  signIn(email: string, password: string) {
    
  }

  refreshToken(){

  }
}
