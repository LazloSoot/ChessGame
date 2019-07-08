import { NgModule } from "@angular/core";
import { SharedModule } from "../../shared/shared.module";
import { PuzzlesRoutingModule } from "./puzzles-routing.module";
import { PuzzlesComponent } from "./puzzles.component";
import { AuthGuard } from "../../core";

@NgModule({
	imports: [SharedModule, PuzzlesRoutingModule],
	declarations: [PuzzlesComponent],
	providers: [AuthGuard]
})
export class PuzzlesModule {}
