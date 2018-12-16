import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PuzzlesComponent } from "./puzzles.component";
import { AuthGuard } from "../../core";

const routes: Routes = [
	{
		path: "",
		component: PuzzlesComponent,
		canActivate: [AuthGuard]
	}
];
@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [AuthGuard]
})
export class PuzzlesRoutingModule {}
