import { NgModule } from "@angular/core";
import { SharedModule } from "../../shared/shared.module";
import { ChessGameRoutingModule } from "./chess-game-routing.module";
import { ChessGameComponent } from "./chess-game.component";
import { AuthGuard } from "../../core";

@NgModule({
	imports: [SharedModule, ChessGameRoutingModule],
	declarations: [ChessGameComponent],
	providers: [AuthGuard]
})
export class ChessGameModule {}
