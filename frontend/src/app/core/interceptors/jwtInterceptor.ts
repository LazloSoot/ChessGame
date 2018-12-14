import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        //add auth header with jwt token if available
        let currentUser = null; // get user from authService or appStateService
        if(currentUser ) { //&& this.authenticationService.token)
            request = request.clone({
                setHeaders: {
                   // Authirization: `Bearer ${this.authService.token}`
                }
            });
        }

        return next.handle(request);
    }
}