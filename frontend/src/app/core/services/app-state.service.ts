import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { User } from '../models';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class AppStateService {
  private tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>("");
  private isLogedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private currentUserSubject: BehaviorSubject<User> = new BehaviorSubject<User>(null);

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

  constructor(
    private router: Router,
    private userService: UserService
  ) { 
    /////// BAD IDEA
    this.isLogedIn = localStorage.getItem("chess-zm-isLogedIn") === "true";
    this.token = localStorage.getItem("chess-zm-token");
  }

  getCurrentUser(): Observable<User> {
    return this.currentUserSubject.asObservable();
  }

  async updateAuthState(authState: Observable<firebase.User>, currentUser: User, isRemember: boolean): Promise<void> {
    if(authState)
    {
	  authState.subscribe((currentUser) => this.listenAuthState(currentUser));

    this.currentUserSubject.next(currentUser);

	  this.isLogedIn = true;
      if(isRemember) {
        /////// BAD IDEA
        localStorage.setItem("chess-zm-isLogedIn", "true");
        localStorage.setItem("chess-zm-token", this.token);
      }
	}
  }
  
  private async initializeCurrentUser(firebaseUser: firebase.User): Promise<void> {
	  debugger;
	  // userInfo.providerId === "password" means that user logged in by email and password
	  // i.e we need to take a data like avatarUrl and name from our db
	  if(firebaseUser.providerData.filter(userInfo => userInfo.providerId === "password").length > 0)
	  {
		return await this.userService.get(firebaseUser.uid)
      		.toPromise()
      		.then(async user => {
				  debugger;
      		  if(user) {
					this.currentUserSubject.next(user);
      		  } else {
				debugger;
      		    throw new Error(`There is no such user in db!`);
      		  }
			})
			.catch(error => {
				debugger;
				 throw error; } )  ;
	  } else {
		  let u: User = {
			  id: undefined,
			  uid: firebaseUser.uid,
			  name: firebaseUser.displayName,
			  avatarUrl: firebaseUser.photoURL
		  }
		  this.currentUserSubject.next(u);
	  }
  }

  private listenAuthState(currentUser: firebase.User | null) {
    if(!currentUser){
		this.currentUserSubject.next(null);
      	this.token = null;
      	this.isLogedIn = false;
      	this.router.navigate(["/"]);
    }
  }
}
