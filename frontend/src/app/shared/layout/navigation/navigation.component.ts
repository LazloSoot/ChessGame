import { Component, OnInit, ChangeDetectorRef, NgZone } from "@angular/core";
import { MediaMatcher } from "@angular/cdk/layout";
import { EventService } from "../../helpers";
import { MatDialog } from "@angular/material";
import { SignInDialogComponent } from "../../dialogs/sign-in-dialog/sign-in-dialog.component";
import { SignUpDialogComponent } from "../../dialogs/sign-up-dialog/sign-up-dialog.component";
import { AppStateService, User, AuthService } from "../../../core";
import { Router } from "@angular/router";

@Component({
	selector: "app-navigation",
	templateUrl: "./navigation.component.html",
	styleUrls: ["./navigation.component.less"]
})
export class NavigationComponent implements OnInit {
	mobileQuery: MediaQueryList;
	private _mobileQueryListener: () => void;
	private user: User;
	private defaultAvatarUrl ="../../../../assets/images/anonAvatar.png";
	private _isLoggedIn: boolean;
	
	constructor(
		private zone: NgZone,
		private changeDetectorRef: ChangeDetectorRef,
		private media: MediaMatcher,
		private eventService: EventService,
		private dialog: MatDialog,
		private router: Router,
		private appStateService: AppStateService,
		private authService: AuthService
	) {
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
		
		this.appStateService.getCurrentUserObs()
		.subscribe((user) => {
			if(user){
				this.user = user;
				this.zone.run(() => {
					this.router.navigate(['/play']);
				});
			}
			else {
				this.user = new User("not signed", "../../../../assets/images/anonAvatar.png");
				this.router.navigate(['/']);
			}
		});
	}

	ngOnDestroy() {
		this.mobileQuery.removeListener(this._mobileQueryListener);
	}

	isLoggedIn(): boolean {
		if(this.appStateService.getCurrentUser()) {
			return true;
		} else {
			return false;
		}
	}

	onSignUpClick() {
		let dialogRef = this.dialog.open(SignUpDialogComponent);

		dialogRef.componentInstance.onSuccessSignUp.subscribe(
			(isSuccess) => {
				if(isSuccess) {
					this.router.navigate(['/play']);
				}
			}
		)

		dialogRef.afterClosed().subscribe(() => {
			dialogRef.componentInstance.onSuccessSignUp.unsubscribe();
		});
		
	}

	onLoginClick() {
		let dialogRef = this.dialog.open(SignInDialogComponent);

        dialogRef.componentInstance.onSucceessLogin.subscribe(
            (isSuccess) => {
				if(isSuccess) {
					this.router.navigate(['/play']);
				}
            }
		);
		
		dialogRef.afterClosed().subscribe(() => {
			dialogRef.componentInstance.onSucceessLogin.unsubscribe();
		});
	}

	onLogoutClick() {
		this.authService.logout();
	}
}
