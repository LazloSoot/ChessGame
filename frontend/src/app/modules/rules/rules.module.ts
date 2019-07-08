import { NgModule } from "@angular/core";
import { SharedModule } from "../../shared/shared.module";
import { RulesRoutingModule } from "./rules-routing.module";
import { RulesComponent } from "./rules.component";
import { AuthGuard } from "../../core";

@NgModule({
	imports: [SharedModule, RulesRoutingModule],
	declarations: [RulesComponent],
	providers: [AuthGuard]
})
export class RulesModule {}
