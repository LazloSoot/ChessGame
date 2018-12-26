import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, from, of } from 'rxjs';
import { catchError, flatMap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class HttpService {
    private url = environment.apiUrl;
    constructor(private httpClient: HttpClient) { }

   //sendRequest(
   //    type: RequestMethod,
   //    endpoint: string,
   //    params: number | string = "",
   //    body: any = {},
   //    respType: string = 'json',
   //    typeOfContent: string = "json") {
   //    return this.createRequest(type, endpoint, params, body, respType, typeOfContent).pipe(
   //        catchError((res: HttpErrorResponse) => this.handleError(res)),
   //        flatMap((response: any) => {
   //            if (response === "T") {
   //                return from(this.authService.refreshToken()).pipe(flatMap(
   //                    () => this.createRequest(type, endpoint, params, body, respType, typeOfContent)
   //                        .pipe(
   //                            catchError((res: HttpErrorResponse) => this.handleError(res)),
   //                            flatMap((response: any) => {
   //                                if (response === "T") {
   //                                    return this.authService.logout()
   //                                } else {
   //                                    return of(response);
   //                                }
   //                            })
   //                        )
   //                ))
   //            } else {
   //                return of(response);
   //            }
   //        })
   //    );
   //}

   sendRequest(
    type: RequestMethod,
    endpoint: string,
    params: number | string = "",
    body: any = {},
    respType: string = 'json',
    typeOfContent: string = "json") {
    return this.createRequest(type, endpoint, params, body, respType, typeOfContent).pipe(
        flatMap((response: any) => {
            return of(response);
        })
    );
}

    createRequest(type: RequestMethod,
        endpoint: string,
        params: number | string = "",
        body: any = {},
        respType: string = 'json',
        typeOfContent: string = "json") {

        let headers = new HttpHeaders({ 'Access-Control-Allow-Origin': '*'});
        if ((type === RequestMethod.Post || type === RequestMethod.Put) && typeOfContent == 'json') {
            headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        }
        let request: Observable<any>;

        switch (type) {
            case RequestMethod.Get:
                if (respType === 'json') {
                    request = this.httpClient.get(`${this.url}/${endpoint}/${params}`, { responseType: 'json', headers });
                } else if (respType === 'blob') {
                    request = this.httpClient.get(`${this.url}/${endpoint}/${params}`, { responseType: 'blob', headers });
                }
                break;
            case RequestMethod.Post:
                request = this.httpClient.post(`${this.url}/${endpoint}/${params}`, body, { headers });
                break;
            case RequestMethod.Put:
                request = this.httpClient.put(`${this.url}/${endpoint}/${params}`, body, { headers });
                break;
            case RequestMethod.Delete:
                const httpOptions = {
                    headers: headers, body: body
                };
                request = this.httpClient.delete(`${this.url}/${endpoint}/${params}`, httpOptions);
                break;
        }

        return request;
    }
}

export enum RequestMethod {
    Get,
    Post,
    Put,
    Delete,
}