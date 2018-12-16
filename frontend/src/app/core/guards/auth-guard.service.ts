import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AppStateService } from '../services/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {

  constructor(private router: Router, private appState: AppStateService) { }


    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if(this.appState.isLogedIn)
        {
          return true;
        }
        else {
          this.router.navigate(['/']);
          return true;
        }
    }
}
