import { NgModule } from "@angular/core";
import { ProfileComponent } from "./profile.component";
import { SharedModule } from "../../shared/shared.module";
import { ProfileRoutingModule } from "./profile-routing.module";
import { AuthGuard } from "../../core";

@NgModule({
	imports: [SharedModule, ProfileRoutingModule],
	declarations: [ProfileComponent],
	providers: [AuthGuard]
})
export class ProfileModule {}
