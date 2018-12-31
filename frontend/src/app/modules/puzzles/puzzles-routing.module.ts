import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PuzzlesComponent } from "./puzzles.component";

const routes: Routes = [
	{
		path: "",
		component: PuzzlesComponent
	}
];
@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})
export class PuzzlesRoutingModule {}
