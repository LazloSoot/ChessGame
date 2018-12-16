import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChessGameRoutingModule } from './chess-game-routing.module';
import { ChessGameComponent } from './chess-game.component';
import { AuthGuard } from '../../core';

@NgModule({
  imports: [
    CommonModule,
    ChessGameRoutingModule
  ],
  declarations: [ ChessGameComponent ],
  providers: [AuthGuard]
})
export class ChessGameModule { }
