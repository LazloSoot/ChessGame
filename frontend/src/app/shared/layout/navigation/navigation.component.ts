import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { MediaMatcher } from "@angular/cdk/layout";
import { EventService } from "../../helpers";

@Component({
	selector: "app-navigation",
	templateUrl: "./navigation.component.html",
	styleUrls: ["./navigation.component.less"]
})
export class NavigationComponent implements OnInit {
	mobileQuery: MediaQueryList;
	private _mobileQueryListener: () => void;

	constructor(
		private changeDetectorRef: ChangeDetectorRef,
		private media: MediaMatcher,
		private eventService: EventService
	) {}
  
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
	}

	ngOnDestroy() {
		this.mobileQuery.removeListener(this._mobileQueryListener);
	}

	isLoggedIn() {
		return false;
	}

	onSignUpClick() {
		console.log("onSignUp");
	}

	onLoginClick() {
		console.log("onLogin");
	}

	onLogoutClick() {
		console.log("onLogout");
	}
}
