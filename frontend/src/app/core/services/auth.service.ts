import { Injectable } from "@angular/core";
import { AngularFireAuth } from "@angular/fire/auth";
import * as firebase from "firebase/app";
import { from } from "rxjs";
import { AuthProviderType, User } from "../models";
import { AppStateService } from "./app-state.service";
import { UserService } from "./user.service";

// aSfg14Gszs
@Injectable({
	providedIn: "root"
})
export class AuthService {
	private isRemember: boolean;
	constructor(
		private firebaseAuth: AngularFireAuth,
		private appStateService: AppStateService,
		private userService: UserService
	) {
		this.isRemember = appStateService.isRemember;
		this.firebaseAuth.auth.onAuthStateChanged(
			async firebaseUser => {
				if(firebaseUser) {
					if(this.isRemember) {
						await this.appStateService.updateAuthState(
							firebaseUser,
							true
						)
					}
				} else {
					await this.appStateService.updateAuthState(
						firebaseUser
					)
				}
		});
	}

	signUpRegular(email: string, userName: string, password: string) {
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
						.then(async () => {

							await this.appStateService.updateAuthState(
								userCred.user,
								true
							).then(async () => {
								await this.userService
									.add(new User(userCred.user.uid, userName))
									.toPromise();
							})
							//throw new Error(
							//	`You need to confirm your email address in order to use our service. Email confirmation was already send to ${
							//		userCred.user.email
							//	}. Check your email.`
							//);
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

	signInRegular(email: string, password: string, isRemember?: boolean) {
		return from(
			this.firebaseAuth.auth.signInWithEmailAndPassword(email, password)
		)
			.toPromise()
			.then(
				async userCred => {
					if (userCred.user.emailVerified) {
						await this.appStateService.updateAuthState(
							userCred.user,
							isRemember
						);
					} else {
						await userCred.user
							.sendEmailVerification()
							.then(() => {
								throw new Error(
									`You need to confirm your email address in order to use our service.Email confirmation was already send to ${
										userCred.user.email
									}. Please check your email.`
								);
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

	signIn(authProviderType: AuthProviderType, isRemember?: boolean) {
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
						userCred.user,
						isRemember
					);
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
