import { Injectable } from "@angular/core";
import { AngularFireAuth } from "@angular/fire/auth";
import * as firebase from "firebase/app";
import { from } from "rxjs";
import { AuthProviderType, User, EmailNotVerifiedError } from "../models";
import { AppStateService } from "./app-state.service";
import { UserService } from "./user.service";
import { reject } from "q";
import { PresenceService } from "./presence.service";
import { AngularFireDatabase } from "@angular/fire/database";

@Injectable({
	providedIn: "root"
})

//  asGD1234sfgs
export class AuthService {
	private isRemember: boolean;
	private currentUserCredintials: firebase.auth.UserCredential;

	public get isUserLogedIn() {
		return this.currentUserCredintials !== undefined && this.currentUserCredintials !== null;
	}

	constructor(
		private firebaseAuth: AngularFireAuth,
		private database: AngularFireDatabase,
		private appStateService: AppStateService,
		private userService: UserService,
		private presenceService: PresenceService
	) {
		this.isRemember = appStateService.isRemember;
		this.firebaseAuth.auth.onAuthStateChanged(
			async firebaseUser => {
				if(firebaseUser) {
					if(this.isRemember && (this.checkProviderData(firebaseUser))) {
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

	signUpRegular(email: string, userName: string, password: string): Promise<void> {
		return from(
			this.firebaseAuth.auth.createUserWithEmailAndPassword(
				email,
				password
			)
		)
			.toPromise()
			.then(async userCredential => {  // add new user to db
				const dateNow = new Date(userCredential.user.metadata.creationTime);
				await this.userService
						.add( {
							name: userName,
							uid: userCredential.user.uid,
							registrationDate: dateNow,
							avatarUrl: undefined,
							id: undefined,
							isOnline: undefined,
							lastSeenDate: dateNow
						})
						.toPromise();

				this.currentUserCredintials = userCredential;
			});
	}

	signInRegular(email: string, password: string, isRemember?: boolean) :Promise<void> {
		return from(
			this.firebaseAuth.auth.signInWithEmailAndPassword(email, password)
		)
			.toPromise()
			.then(
				async userCred => {
					this.currentUserCredintials = userCred;
					if (userCred.user.emailVerified) {
						firebase.database().goOnline();
						return await this.appStateService.updateAuthState(
							userCred.user,
							isRemember
						);
						
					} else {
						throw new EmailNotVerifiedError(
							`You need to confirm your email address in order to use our service.`
						);
					}
				},
				error => {
					throw error;
				}
			)
			.catch(error => {
				throw error;
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
					firebase.database().goOnline();
					await this.appStateService.updateAuthState(
						userCred.user,
						isRemember
					);
				},
				error => {
					return error;
				}
			)
			.catch(error => {
				return error;
			});
	}

	refreshToken() {}

	async sendEmailVerification(): Promise<string> {
		if(this.currentUserCredintials) {
			return await this.currentUserCredintials.user.sendEmailVerification()
			.then(() => {
				return this.currentUserCredintials.user.email
			});
		} else {
			reject("Error occurred while sending email verification.There is no valid user credentials, please contact with administrator.");
		}
	}

	logout() {
		firebase.database().goOffline();
		return from(this.firebaseAuth.auth.signOut());
	}

	private checkProviderData(firebaseUser: firebase.User) : boolean {
		return firebaseUser.providerData.find(pd => pd.providerId !== 'password') !== undefined || (firebaseUser.emailVerified);
	}
}
