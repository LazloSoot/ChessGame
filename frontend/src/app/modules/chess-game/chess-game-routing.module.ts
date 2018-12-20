import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from '../../core';
import { ChessGameComponent } from "./chess-game.component";

const routes: Routes = [
	{
		path: '',
		component: ChessGameComponent,
		//canActivate: [ AuthGuard ]
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [AuthGuard]
})
export class ChessGameRoutingModule {}
