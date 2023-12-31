import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs';
import { BaseResponse } from '../models/base.response';
import { ShortActionModel } from '../models/short.action.model';
import { ActionModel } from '../models/action.model';
import { ExpressionModel } from '../models/expression.model';

@Injectable({
  providedIn: 'root'
})
export class ActionApiService extends BaseApiService {
  constructor(router: Router, private http: HttpClient) { 
    super(router);
  }

  public getAbleEvents() {
    return this.http.get<BaseResponse<string[]>>('/api/actions/able-events').pipe(catchError(e => this.handleError<string[]>(e)));
  }

  public getAction(id: string) {
    return this.http.get<BaseResponse<ActionModel>>(`/api/actions/${id}`).pipe(catchError(e => this.handleError<ActionModel>(e)));
  }

  public getActionExpressions(id: string) {
    return this.http.get<BaseResponse<ExpressionModel>>(`/api/actions/${id}/expressions`).pipe(catchError(e => this.handleError<ExpressionModel>(e)));
  }

  public getActions() {
    return this.http.get<BaseResponse<ShortActionModel[]>>('/api/actions').pipe(catchError(e => this.handleError<ShortActionModel[]>(e)));
  }

  public createAction(action: ActionModel) {
    return this.http.post<BaseResponse<ActionModel>>('/api/actions', ActionModel.toRequestModel(action)).pipe(catchError(e => this.handleError<ActionModel>(e)));
  }

  public updateAction(action: ActionModel) {
    return this.http.put<BaseResponse<ActionModel>>(`/api/actions/${action.id}`, ActionModel.toRequestModel(action)).pipe(catchError(e => this.handleError<ActionModel>(e)));
  }
}
