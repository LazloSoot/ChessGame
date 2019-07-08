import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<any> {
        return next.handle(request).pipe(catchError((err: HttpErrorResponse) => {
            debugger;
            if (err.status === 401) {
                // auto logout if 401 response returned from api
                
              //  this.authenticationService.logout();
            //    location.reload(true);
            } else if (err.status === 404) {

            }
            //const error = err.error || err.message || err.statusText;
            return throwError(err);
        }))
    }
}