import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { LandingComponent } from "./landing.component";
import { LandingGuard } from "../../core";

const routes: Routes = [
	{
		path: "",
		component: LandingComponent,
		canActivate: [LandingGuard]
	}
];
@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})
export class LandingRoutingModule {}
