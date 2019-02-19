import { NgModule } from "@angular/core";
import { ProfileComponent } from "./profile.component";
import { SharedModule } from "../../shared/shared.module";
import { ProfileRoutingModule } from "./profile-routing.module";
import { AuthGuard } from "../../core";
import { CommonModule } from "@angular/common";
import { ActivityComponent } from './activity/activity.component';
import { GamesHistoryComponent } from './games-history/games-history.component';

@NgModule({
	imports: [CommonModule, SharedModule, ProfileRoutingModule],
	declarations: [ProfileComponent, ActivityComponent, GamesHistoryComponent],
	providers: [AuthGuard]
})
export class ProfileModule {}
