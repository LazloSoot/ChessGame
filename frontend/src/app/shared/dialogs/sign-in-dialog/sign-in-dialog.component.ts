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
	@Output() onSucceessLogin = new EventEmitter<any>();
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
			password: ""
		};
	}

	signInWithGoogle() {
		this.authService.signIn(AuthProviderType.Google).then(error => {
			if (error) {
				this.firebaseError = error.message;
			} else {
				this.onSucceessLogin.emit(true);
				this.dialogRef.close();
			}
		});
	}

	signInWithFacebook() {
		this.authService.signIn(AuthProviderType.Facebook).then(error => {
			if (error) {
				this.firebaseError = error.message;
			} else {
				this.onSucceessLogin.emit(true);
				this.dialogRef.close();
			}
		});
	}

	onForgotPasswordClick() {
		this.dialogRef.close();
	}

	onLoginFormSubmit(user, form) {
		debugger;
	}

	onSignUp() {
		this.dialogRef.close();
		setTimeout(() => {
			this.eventService.filter("signUp");
		}, 50);
	}
}
