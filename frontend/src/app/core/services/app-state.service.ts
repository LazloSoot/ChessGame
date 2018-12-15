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
    /////// BAD PRACTICE
    this.isLogedIn = localStorage.getItem("chess-zm-isLogedIn") === "true";
    this.token = localStorage.getItem("chess-zm-token");
  }

  getFirebaseUser(): Observable<firebase.User> {
    return this.firebaseUserSubject.asObservable();
  }

  updateAuthState(authState: Observable<firebase.User>, firebaseUser: firebase.User, token: string, isRemember: boolean) {
    if(authState)
    {
      authState.subscribe((currentUser) => this.listenAuthState(currentUser));
    }
    this.firebaseUserSubject.next(firebaseUser);
    this.token = token;
    this.isLogedIn = true;

    if(isRemember) {

      /////// BAD PRACTICE
      localStorage.setItem("chess-zm-isLogedIn", "true");
      localStorage.setItem("chess-zm-token", token);
    }
  }

  private listenAuthState(currentUser: firebase.User | null) {
    if(!currentUser){
      this.firebaseUserSubject.next(null);
      this.token = null;
      this.isLogedIn = false;
      console.log("LOGED OUT");
    }
  }
}
