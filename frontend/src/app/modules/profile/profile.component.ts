import { Component, OnInit } from "@angular/core";
import { User, UserService, AppStateService } from "../../core";
import { ActivatedRoute } from "@angular/router";

@Component({
	selector: "app-profile",
	templateUrl: "./profile.component.html",
	styleUrls: ["./profile.component.less"]
})
export class ProfileComponent implements OnInit {
  public userProfile: User;
  private isActivitiesLoading: boolean = true;
  private isGamesLoading: boolean = true;
	constructor(
		private route: ActivatedRoute,
		public userService: UserService,
		public appStateService: AppStateService
	) {}

	ngOnInit() {
		this.route.params.subscribe(data => {
			if (data && data.userId) {
				this.userService.get(data.userId).subscribe(user => {
					this.userProfile = user;
					if (!this.userProfile.avatarUrl) {
						this.userProfile.avatarUrl =
							"../../../../assets/images/anonAvatar.png";
					}
				});
			} else {
				this.userProfile = this.appStateService.getCurrentUser();
				this.userService.getCurrentUser().subscribe(currentDbUser => {
					this.userProfile.avatarUrl = (currentDbUser.avatarUrl) ? currentDbUser.avatarUrl : "../../../../assets/images/anonAvatar.png";
				});
			}
    });
    
    setTimeout(() => {
      this.isActivitiesLoading = false;
      this.isGamesLoading = false;
    }, 2000);
	}
}