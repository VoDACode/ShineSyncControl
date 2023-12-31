import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs';
import { UserModel } from '../models/user.model';
import { BaseResponse } from '../models/base.response';

@Injectable({
  providedIn: 'root'
})
export class UserApiService extends BaseApiService {
  constructor(router: Router, private http: HttpClient) {
    super(router);
  }

  public getSelf() {
    return this.http.get<BaseResponse<UserModel>>('/api/users/self').pipe(catchError(e => this.handleError<UserModel>(e)));
  }
}
