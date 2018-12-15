import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppStateService {
  private tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>("");
  private isLogedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private firebaseUserSubject: BehaviorSubject<firebase.User> = new BehaviorSubject<firebase.User>(null);

	public get token(): string {
		return this.tokenSubject.value;
  }
  public set token(value: string) {
    this.tokenSubject.next(value);
  }

  public get isLogedIn(): boolean {
    return this.isLogedInSubject.value;
  }
  public set isLogedIn(value: boolean) {
    if(!value){
      localStorage.removeItem("chess-zm-isLogedIn");
    }

    this.isLogedInSubject.next(value);
  }

  constructor() { 
    this.isLogedIn = localStorage.getItem("chess-zm-isLogedIn") === "true";
    this.token = localStorage.getItem("chess-zm-token");
  }

  getFirebaseUser(): Observable<firebase.User> {
    return this.firebaseUserSubject.asObservable();
  }

  updateAuthState(firebaseUser?: firebase.User, token?: string, loginStatus?: boolean, isRemember?: boolean) {
    this.firebaseUserSubject.next(firebaseUser);
    this.token = token;
    this.isLogedIn = loginStatus;

    if(isRemember) {
      localStorage.setItem("chess-zm-isLogedIn", "true");
      localStorage.setItem("chess-zm-token", token);
    }
  }
}
