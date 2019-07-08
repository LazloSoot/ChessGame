import { Injectable } from "@angular/core";
import {
	Router,
	CanActivate,
	ActivatedRouteSnapshot,
	RouterStateSnapshot
} from "@angular/router";
import { AppStateService } from "../services/app-state.service";

@Injectable({
	providedIn: "root"
})
export class LandingGuard implements CanActivate {
	constructor(
		private router: Router,
		private appStateService: AppStateService
	) {}

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
		if (this.appStateService.getCurrentUser()) {
			this.router.navigate(["/play"]);
			return false;
		} else {
			return true;
		}
	}
}
