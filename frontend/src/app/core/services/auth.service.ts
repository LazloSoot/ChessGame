import { Injectable } from "@angular/core";
import { AngularFireAuth } from "@angular/fire/auth";
import * as firebase from "firebase/app";
import { from } from "rxjs";
import { AuthProviderType, User } from "../models";
import { AppStateService } from "./app-state.service";
import { UserService } from './user.service';

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

						this.appStateService.token = await userCred.user.getIdToken();
						await this.appStateService.updateAuthState(
							this.firebaseAuth.authState,
							await this.initializeCurrentUser(userCred.user),
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
					this.appStateService.token = await userCred.user.getIdToken();
					await this.appStateService.updateAuthState(
						this.firebaseAuth.authState,
						await this.initializeCurrentUser(userCred.user),
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

	private async initializeCurrentUser(firebaseUser: firebase.User): Promise<User> {
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
					  return user;
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
			return u;
		}
	}
}
