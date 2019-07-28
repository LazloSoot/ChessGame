import { NgModule } from "@angular/core";
import { SharedModule } from "../../shared/shared.module";
import { ChessGameRoutingModule } from "./chess-game-routing.module";
import { ChessGameComponent } from "./chess-game.component";
import { AuthGuard } from "../../core";
import { ChessBoardComponent } from './chess-board/chess-board.component';
import { CommonModule } from "@angular/common";
import { MoveTrackerComponent } from './move-tracker/move-tracker.component';
import { NewGameComponent } from './new-game/new-game.component';
import { SettingsComponent } from './settings/settings.component';

@NgModule({
	imports: [CommonModule, SharedModule, ChessGameRoutingModule],
	declarations: [ChessGameComponent, ChessBoardComponent, MoveTrackerComponent, NewGameComponent, SettingsComponent ],
	providers: [AuthGuard]
})
export class ChessGameModule {}
