import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { ProfileSettingsComponent } from "./profile-settings.component";
import { AuthGuard } from "../../../core";

const routes: Routes = [
	{
		path: '',
		component: ProfileSettingsComponent,
		canActivate: [AuthGuard]
	}
];
@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [AuthGuard]
})
export class ProfileSettingsRoutingModule {}
