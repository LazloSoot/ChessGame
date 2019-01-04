import { Injectable } from "@angular/core";
import {
	Router,
	ActivatedRouteSnapshot,
	RouterStateSnapshot,
	CanActivate
} from "@angular/router";
import { AppStateService } from "../services/app-state.service";

@Injectable({
	providedIn: "root"
})
export class AuthGuard implements CanActivate {
	constructor(private router: Router, private appState: AppStateService) {}

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
		if (this.appState.getCurrentUser()) {
			return true;
		} else {
			this.router.navigate(["/"]);
			return false;
		}
	}
}
