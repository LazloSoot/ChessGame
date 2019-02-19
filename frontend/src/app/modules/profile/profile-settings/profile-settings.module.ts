import { NgModule } from "@angular/core";
import { ProfileSettingsRoutingModule } from "./profile-settings-routing.module";
import { SharedModule } from "../../../shared/shared.module";
import { ProfileSettingsComponent } from "./profile-settings.component";
import { AuthGuard } from "../../../core";

@NgModule({
	imports: [SharedModule, ProfileSettingsRoutingModule],
	declarations: [ProfileSettingsComponent],
	providers: [AuthGuard]
})
export class ProfileSettingsModule {}
