import { Component, OnInit, EventEmitter, Output } from "@angular/core";
import { MatDialogRef } from "@angular/material";
import { EventService } from "../../helpers";
import { AuthService, AuthProviderType } from "../../../core";

@Component({
	selector: "app-sign-in-dialog",
	templateUrl: "./sign-in-dialog.component.html",
	styleUrls: ["./sign-in-dialog.component.less"]
})
export class SignInDialogComponent implements OnInit {
	@Output() onSucceessLogin = new EventEmitter<boolean>();
	public firebaseError: string;
	public hide = true;
	public user: any;

	constructor(
		private dialogRef: MatDialogRef<SignInDialogComponent>,
		private eventService: EventService,
		private authService: AuthService
	) {}

	ngOnInit() {
		this.user = {
			login: "",
			password: "",
			isRemember: true
		};
	}

	signInWithGoogle() {
		this.authService.signIn(AuthProviderType.Google, this.user.isRemember)
		.then(error => {
			if (error) {
				this.firebaseError = (error.message) ? error.message : error;
			} else {
				this.onSucceessLogin.emit(true);
				this.dialogRef.close();
			}
		});
	}

	signInWithFacebook() {
		this.authService.signIn(AuthProviderType.Facebook, this.user.isRemember)
		.then(error => {
			if (error) {
				this.firebaseError = (error.message) ? error.message : error;
			} else {
				this.onSucceessLogin.emit(true);
				this.dialogRef.close();
			}
		});
	}

	
	onLoginFormSubmit(user, form) {
		debugger;
		if (form.valid) {
			this.authService.signInRegular(user.login, user.password, user.isRemember)
			.then(error => {
				if(error){
					this.firebaseError = (error.message) ? error.message : error;
				} else {
					this.onSucceessLogin.emit(true);
					this.dialogRef.close();
				}
			})
			.catch(error => {
				this.firebaseError = (error.message) ? error.message : error;
			})
		 }
	}

	onForgotPasswordClick() {
		this.dialogRef.close();
	}


	onSignUp() {
		this.dialogRef.close();
		setTimeout(() => {
			this.eventService.filter("signUp");
		}, 50);
	}
}
