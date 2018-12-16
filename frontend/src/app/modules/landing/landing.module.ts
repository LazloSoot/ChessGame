import { NgModule } from "@angular/core";
import { LandingComponent } from "./landing.component";
import { LandingRoutingModule } from "./landing-routing.module";
import { SharedModule } from "../../shared/shared.module";
import { LandingGuard } from "../../core";

@NgModule({
	imports: [SharedModule, LandingRoutingModule],
	declarations: [LandingComponent],
	providers: [LandingGuard]
})
export class LandingModule {}
