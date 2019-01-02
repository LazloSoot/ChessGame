import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { Router } from "@angular/router";
import { User, UserConnection, Group, Hub } from "../models";
import { SignalRService } from "./signalr.service";

@Injectable({
	providedIn: "root"
})
export class AppStateService {
	private tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>("");
	private isLogedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
	private currentUserSubject: BehaviorSubject<User> = new BehaviorSubject<User>(null);
	private _signalRConnection: UserConnection;

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
		if (!value) {
			localStorage.removeItem("chess-zm-isLogedIn");
		}

		this.isLogedInSubject.next(value);
	}

	public get signalRConnection(): UserConnection {
		return this._signalRConnection;
	}

	constructor(
		private router: Router,
		private signalRService: SignalRService
	) {
		/////// BAD IDEA
		this.isLogedIn = localStorage.getItem("chess-zm-isLogedIn") === "true";
		this.token = localStorage.getItem("chess-zm-token");
	}

	getCurrentUserObs(): Observable<User> {
		return this.currentUserSubject.asObservable();
	}

	getCurrentUser(): User {
		return this.currentUserSubject.value;
	}

	async updateAuthState(
		authState: Observable<firebase.User>,
		currentUser: User,
		isRemember: boolean
	): Promise<void> {
		if (authState) {
			authState.subscribe(currentUser =>
				this.listenAuthState(currentUser)
			);

			this.currentUserSubject.next(currentUser);

			this.isLogedIn = true;
			if (isRemember) {
				/////// BAD IDEA
				localStorage.setItem("chess-zm-isLogedIn", "true");
				localStorage.setItem("chess-zm-token", this.token);
			}
		}
	}

	private listenAuthState(currentUser: firebase.User | null) {
		debugger;
		if (!currentUser) {
			this._signalRConnection.offAll();
			this.signalRService.leaveGroup(
				`${Group.User}${this.currentUserSubject.value.uid}`,
				Hub.Notification
			);
			this.currentUserSubject.next(null);
			this.token = null;
			this.isLogedIn = false;
			this.router.navigate(["/"]);
		} else {
			//this.signalRConnection = 
			this._signalRConnection = this.signalRService.connect(
				`${Group.User}${this.currentUserSubject.value.uid}`,
				Hub.Notification,
				this.token
			);
		}
	}
}
