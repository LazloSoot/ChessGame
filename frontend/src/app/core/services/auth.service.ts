import { Injectable } from "@angular/core";
import { AngularFireAuth } from "@angular/fire/auth";
import * as firebase from "firebase/app";
import { from } from "rxjs";
import { AuthProviderType } from "../models";
import { AppStateService } from "./app-state.service";
import { HttpService } from './http.service';

@Injectable({
	providedIn: "root"
})
export class AuthService {
	constructor(
		private firebaseAuth: AngularFireAuth,
		private appStateService: AppStateService
	) {}

	signUpRegular(email: string, password: string) {
		// aSfg14Gszs
		return from(
			this.firebaseAuth.auth.createUserWithEmailAndPassword(
				email,
				password
			)
		)
			.toPromise()
			.then(
				async userCred => {
					await userCred.user
						.sendEmailVerification()
						.then(() => {
							throw new Error(`You need to confirm your email address in order to use our service. Email confirmation was already send to ${userCred.user.email}. Check your email.`);
						})
						.catch(error => {
							throw error;
						});
				},
				error => {
					return error;
				}
			)
			.catch(error => {
				return error;
			});
	}

	signInRegular(email: string, password: string) {
		return from(
			this.firebaseAuth.auth.signInWithEmailAndPassword(email, password)
		)
			.toPromise()
			.then(
				async userCred => {
					if (userCred.user.emailVerified) {
						await this.appStateService.updateAuthState(
							this.firebaseAuth.authState,
							userCred.user,
							await userCred.user.getIdToken(),
							false
						)
						.catch(error => { throw error; });
					} else {
						await userCred.user
							.sendEmailVerification()
							.then(() => {
								throw new Error(`You need to confirm your email address in order to use our service.Email confirmation was already send to ${userCred.user.email}. Please check your email.`);
							})
							.catch(error => {
								throw error;
							});
					}
				},
				error => {
					return error;
				}
			)
			.catch(error => {
				return error;
			});
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
					await this.appStateService.updateAuthState(
						this.firebaseAuth.authState,
						userCred.user,
						await userCred.user.getIdToken(),
						false
					)
					.catch(error => { 
						debugger; 
						throw error; });
				},
				error => {
					debugger;
					return error;
				}
			)
			.catch(error => {
				debugger;
				return error;
			});
	}

	refreshToken() {}

	logout() {
		return from(this.firebaseAuth.auth.signOut());
	}
}
