import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { MediaMatcher } from "@angular/cdk/layout";
import { EventService } from "../../helpers";
import { MatDialog } from "@angular/material";
import { SignInDialogComponent } from "../../dialogs/sign-in-dialog/sign-in-dialog.component";
import { SignUpDialogComponent } from "../../dialogs/sign-up-dialog/sign-up-dialog.component";
import { AppStateService, SignedUser, AuthService } from "../../../core";

@Component({
	selector: "app-navigation",
	templateUrl: "./navigation.component.html",
	styleUrls: ["./navigation.component.less"]
})
export class NavigationComponent implements OnInit {
	mobileQuery: MediaQueryList;
	private _mobileQueryListener: () => void;
	private user: SignedUser;
	
	constructor(
		private changeDetectorRef: ChangeDetectorRef,
		private media: MediaMatcher,
		private eventService: EventService,
		private dialog: MatDialog,
		private appStateService: AppStateService,
		private authService: AuthService
	) {
		this.user = {
			avatarUrl: "../../../../assets/images/anonAvatar.png",
			nickname: "not signed"
		}
	}
  
	ngOnInit() {
		this.mobileQuery = this.media.matchMedia("(max-width: 960px)");
		this._mobileQueryListener = () =>
			this.changeDetectorRef.detectChanges();
		this.mobileQuery.addListener(this._mobileQueryListener);

		this.eventService.listen().subscribe(event => {
			switch (event) {
				case "signUp":
					this.onSignUpClick();
					break;
				case "login":
					this.onLoginClick();
					break;
			}
		});
		this.appStateService.getFirebaseUser()
		.subscribe((user) => {
			if(user){
				this.user.avatarUrl = user.photoURL;
				this.user.nickname = user.displayName;
			}
			else {
				this.user.avatarUrl = "../../../../assets/images/anonAvatar.png";
				this.user.nickname = "not signed";
			}
		})
	}

	ngOnDestroy() {
		this.mobileQuery.removeListener(this._mobileQueryListener);
	}

	isLoggedIn() {
		return this.appStateService.isLogedIn;
	}

	onSignUpClick() {
		let dialogRef = this.dialog.open(SignUpDialogComponent);

		dialogRef.componentInstance.onSuccessSignUp.subscribe(
			(signUpData) => {
				// получить пользователя
			}
		)

		dialogRef.afterClosed().subscribe(() => {
			dialogRef.componentInstance.onSuccessSignUp.unsubscribe();
		});
		
	}

	onLoginClick() {
		let dialogRef = this.dialog.open(SignInDialogComponent);

        dialogRef.componentInstance.onSucceessLogin.subscribe(
            (loginData) => {
                // получить пользователя
            }
		);
		
		dialogRef.afterClosed().subscribe(() => {
			dialogRef.componentInstance.onSucceessLogin.unsubscribe();
		});
	}

	onLogoutClick() {
		this.authService.logout().subscribe(() => {
			this.appStateService.updateAuthState();
		});
	}
}
