import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../core";
import { ProfileComponent } from "./profile.component";

const routes: Routes = [
	{
		path: '',
		component: ProfileComponent,
		canActivate: [AuthGuard]
	},
	{
		path: 'settings',
		loadChildren: './profile-settings/profile-settings.module#ProfileSettingsModule'
	},
	{
		path: ':userId',
		component: ProfileComponent
	}
];
@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [AuthGuard]
})
export class ProfileRoutingModule {}
