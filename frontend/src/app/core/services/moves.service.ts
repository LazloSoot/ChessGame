import { Injectable } from '@angular/core';
import { HttpService, RequestMethod } from './http.service';
import { MoveRequest, Move } from '../models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MovesService {
  private _apiUrl: string = "/moves";
  constructor(private httpService: HttpService) {}
  
  public commitMove(moveRequest: MoveRequest): Observable<Move> {
    return this.httpService.sendRequest(RequestMethod.Post, this._apiUrl, undefined, moveRequest);
  }
}
