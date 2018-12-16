import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PuzzlesRoutingModule } from './puzzles-routing.module';
import { PuzzlesComponent } from './puzzles.component';
import { AuthGuard } from '../../core';

@NgModule({
  imports: [
    CommonModule,
    PuzzlesRoutingModule
  ],
  declarations: [ PuzzlesComponent ],
  providers: [AuthGuard]
})
export class PuzzlesModule { }
