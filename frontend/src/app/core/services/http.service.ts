import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, from, of } from 'rxjs';
import { catchError, flatMap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { HttpParamsOptions } from '@angular/common/http/src/params';

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
    namedParams?:  { [paramName: string]: string; },
    body: any = {},
    respType: string = 'json',
    typeOfContent: string = "json"
    ) {
    return this.createRequest(type, endpoint, params, namedParams, body, respType, typeOfContent).pipe(
        flatMap((response: any) => {
            return of(response);
        })
    );
}

    createRequest(type: RequestMethod,
        endpoint: string,
        params: number | string,
        namedParams?:  { [paramName: string]: string; },
        body: any = {},
        respType: string = 'json',
        typeOfContent: string = "json"
        ) {

        let headers = new HttpHeaders();
        if ((type === RequestMethod.Post || type === RequestMethod.Put) && typeOfContent == 'json') {
            headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        }
        headers.append('Access-Control-Allow-Origin', '*');
        let request: Observable<any>;
        let requestParams: HttpParams;
        debugger;
        if(namedParams) {
            requestParams = new HttpParams();
            for(let param in namedParams) {
                requestParams = requestParams.append(param, namedParams[param]);
            }
        }

        switch (type) {
            case RequestMethod.Get:
                if (respType === 'json') {
                    request = this.httpClient.get(`${this.url}/${endpoint}/${params}`, { responseType: 'json', headers, params: requestParams });
                } else if (respType === 'blob') {
                    request = this.httpClient.get(`${this.url}/${endpoint}/${params}`, { responseType: 'blob', headers, params: requestParams  });
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