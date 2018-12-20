import { NgModule } from "@angular/core";
import { SharedModule } from "../../shared/shared.module";
import { ChessGameRoutingModule } from "./chess-game-routing.module";
import { ChessGameComponent } from "./chess-game.component";
import { AuthGuard } from "../../core";
import { ChessBoardComponent } from './chess-board/chess-board.component';
import { CommonModule } from "@angular/common";

@NgModule({
	imports: [CommonModule, SharedModule, ChessGameRoutingModule],
	declarations: [ChessGameComponent, ChessBoardComponent ],
	providers: [AuthGuard]
})
export class ChessGameModule {}
