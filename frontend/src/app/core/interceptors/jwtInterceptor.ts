import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppStateService } from '../services';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(
        private appStateService: AppStateService
    ) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        //add auth header with jwt token if available
        if(this.appStateService.token) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${this.appStateService.token}`
                }
            });
        }

        return next.handle(request);
    }
}