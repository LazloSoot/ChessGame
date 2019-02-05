import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { Router } from "@angular/router";
import { User, UserConnection, Group, Hub, GameSettings } from "../models";
import { SignalRService } from "./signalr.service";
import { UserService } from "./user.service";
import * as firebase from "firebase/app";

@Injectable({
	providedIn: "root"
})
export class AppStateService {
	private tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>("");
	private currentUserSubject: BehaviorSubject<User> = new BehaviorSubject<User>(null);
	private currentGameSubject: BehaviorSubject<GameSettings> = new BehaviorSubject<GameSettings>(null); 
	private _signalRConnection: UserConnection;
	private _isRemember: boolean;

	public get token(): string {
		return this.tokenSubject.value;
	}

	public get isRemember(): boolean {
		return this._isRemember;
	}

	public get signalRConnection(): UserConnection {
		return this._signalRConnection;
	}

	public getCurrentUserObs(): Observable<User> {
		return this.currentUserSubject.asObservable();
	}

	public getCurrentUser(): User {
		return this.currentUserSubject.value;
	}

	public get currentGame(): GameSettings {
		return this.currentGameSubject.value;
	}

	public set currentGame(value: GameSettings) {
		this.currentGameSubject.next(value);
	}

	public getCurrentGameObs(): Observable<GameSettings> {
		return this.currentGameSubject.asObservable();
	}

	constructor(
		private userService: UserService,
		private signalRService: SignalRService
	) {
		this._isRemember = localStorage.getItem("chess-zm-isRemember") === "true";
		const currentGame = JSON.parse(localStorage.getItem("chess-zm-currentGame"));
		if(currentGame) {
			this.currentGameSubject.next(currentGame);
		}
		this.currentGameSubject.subscribe((settings) => {
			if(settings)
			{
				localStorage.setItem("chess-zm-currentGame", JSON.stringify(settings));
			}
		})
	}

	async updateAuthState(firebaseUser: firebase.User, isRemember?: boolean): Promise<void> {
		if (firebaseUser) {
			this.tokenSubject.next(await firebaseUser.getIdToken());
			this.currentUserSubject.next(
				await this.initializeCurrentUser(firebaseUser)
			);

			this._signalRConnection = this.signalRService.connect(
				`${Group.User}${firebaseUser.uid}`,
				Hub.Notification,
				this.token
			);
			if(isRemember) {
				localStorage.setItem("chess-zm-isRemember", "true");
			} else {
				localStorage.removeItem("chess-zm-isRemember");
			}
		} else {
			if (this.signalRConnection) {
				this._signalRConnection.offAll();
				this.signalRService.leaveGroup(
					`${Group.User}${firebaseUser.uid}`,
					Hub.Notification
				);
			}
			this.currentUserSubject.next(null);
			this.tokenSubject.next(null);
		}
	}

	private async initializeCurrentUser(firebaseUser: firebase.User): Promise<User> {
		// userInfo.providerId === "password" means that user logged in by email and password
		// i.e we need to take a data like avatarUrl and name from our db
		if (
			firebaseUser.providerData.length < 2 &&
			firebaseUser.providerData.filter(
				userInfo => userInfo.providerId === "password"
			).length > 0
		) {
			return await this.userService
				.getCurrentUser()
				.toPromise()
				.then(async user => {
					if (user) {
						return user;
					} else {
						throw new Error(`There is no such user in db!`);
					}
				})
				.catch(error => {
					debugger;
					throw error;
				});
		} else {
			let u: User = {
				id: undefined,
				name: firebaseUser.displayName,
				avatarUrl: firebaseUser.photoURL
			};
			return u;
		}
	}
}
