import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RulesComponent } from "./rules.component";
import { AuthGuard } from "../../core";

const routes: Routes = [
	{
		path: "",
		component: RulesComponent,
		canActivate: [AuthGuard]
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [AuthGuard]
})
export class RulesRoutingModule {}
