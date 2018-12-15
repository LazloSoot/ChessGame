import { Injectable } from "@angular/core";
import { AngularFireAuth } from "@angular/fire/auth";
import * as firebase from "firebase/app";
import { from } from "rxjs";
import { AuthProviderType } from "../models";
import { AppStateService } from "./app-state.service";

@Injectable({
	providedIn: "root"
})
export class AuthService {
	constructor(
		private firebaseAuth: AngularFireAuth,
		private appStateService: AppStateService
		) {}

	signUpRegular(email: string, password: string) {
		return from(
			this.firebaseAuth.auth.createUserWithEmailAndPassword(
				email,
				password
			)
		);
	}

	signIn(authProviderType: AuthProviderType) {
		let authProvider;
		switch (authProviderType) {
			case AuthProviderType.Google: {
				authProvider = new firebase.auth.GoogleAuthProvider();
				break;
			}
			case AuthProviderType.Facebook: {
				authProvider = new firebase.auth.FacebookAuthProvider();
				break;
			}
		}

		return from(this.firebaseAuth.auth.signInWithPopup(authProvider))
			.toPromise()
			.then(
				async userCred => {
					this.appStateService.updateAuthState(userCred.user, await userCred.user.getIdToken(), true);
				},
				error => {
					return error;
				}
			);
	}

	signInRegular(email: string, password: string) {
		return from(
			this.firebaseAuth.auth.signInWithEmailAndPassword(email, password)
		);
	}

	refreshToken() {

	}

	logout() {
		return from(this.firebaseAuth.auth.signOut());
	}
}
