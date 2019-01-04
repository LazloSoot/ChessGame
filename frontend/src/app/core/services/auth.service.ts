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
	constructor(
		private firebaseAuth: AngularFireAuth,
		private appStateService: AppStateService,
		private userService: UserService
	) {}

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
							// we initialize token for ability to create record(interceptor need to add token to auth header)
							this.appStateService.token = await userCred.user.getIdToken();
							await this.userService
								.add(new User(userCred.user.uid, userName))
								.toPromise();
							this.appStateService.token = undefined;
							throw new Error(
								`You need to confirm your email address in order to use our service. Email confirmation was already send to ${
									userCred.user.email
								}. Check your email.`
							);
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
						this.appStateService.token = await userCred.user.getIdToken();
						await this.appStateService
							.updateAuthState(
								this.firebaseAuth.authState,
								await this.getCurrentDbUser(userCred.user),
								false
							)
							.catch(error => {
								throw error;
							});
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
					this.appStateService.token = await userCred.user.getIdToken();
					await this.appStateService
						.updateAuthState(
							this.firebaseAuth.authState,
							await this.getCurrentDbUser(userCred.user),
							false
						)
						.catch(error => {
							debugger;
							throw error;
						});
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

	private async getCurrentDbUser(firebaseUser: firebase.User): Promise<User> {
		return await this.userService
			.get(firebaseUser.uid)
			.toPromise()
			.then(user => {
				return user;
			})
			.catch(async error => {
				if (error.status === 404) {
					// userInfo.providerId === "password" means that user logged in by email and password
					// i.e we need to ask user for nickname
					if (
						firebaseUser.providerData.length < 2 &&
						firebaseUser.providerData.filter(
							userInfo => userInfo.providerId === "password"
						).length > 0
					) {
						throw new Error(`There is no such user in db!`);
					} else {
						// we initialize token for ability to create record(interceptor needs to add token to auth header)
						this.appStateService.token = await firebaseUser.getIdToken();
						return await this.userService
							.add(
								new User(
									firebaseUser.uid,
									firebaseUser.displayName,
									firebaseUser.photoURL
								)
							)
							.toPromise()
							.then((user: User) => {
								return user;
							});
					}
				}
				throw error;
			});
	}
}
