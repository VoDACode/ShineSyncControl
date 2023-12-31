import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BaseResponse } from '../models/base.response';
import { ExpressionModel } from '../models/expression.model';
import { catchError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExpressionApiService extends BaseApiService {

  constructor(router: Router, private http: HttpClient) {
    super(router);
  }

  public getExpression(id: string) {
    return this.http.get<BaseResponse<ExpressionModel>>(`api/expressions/${id}`).pipe(catchError(e => this.handleError<ExpressionModel>(e)));
  }

  public updateExpression(expression: ExpressionModel) {
    return this.http.put<BaseResponse<ExpressionModel>>(`api/expressions/${expression.id}`, expression).pipe(catchError(e => this.handleError<ExpressionModel>(e)));
  }

  public createInExpression(rootExpressionId: number, expression: ExpressionModel) {
    return this.http.post<BaseResponse<ExpressionModel>>(`api/expressions/${rootExpressionId}/createin`, expression).pipe(catchError(e => this.handleError<ExpressionModel>(e)));
  }
  
  public deleteExpression(id: number) {
    return this.http.delete<BaseResponse<ExpressionModel>>(`api/expressions/${id}`).pipe(catchError(e => this.handleError<ExpressionModel>(e)));
  }
}
