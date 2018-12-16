import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RulesRoutingModule } from "./rules-routing.module";
import { RulesComponent } from "./rules.component";
import { AuthGuard } from "../../core";

@NgModule({
	imports: [CommonModule, RulesRoutingModule],
	declarations: [RulesComponent],
	providers: [AuthGuard]
})
export class RulesModule {}
