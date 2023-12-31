import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BaseResponse } from '../models/base.response';
import { RequestTaskModel, TaskModel } from '../models/task.model';
import { catchError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskApiService extends BaseApiService {

  constructor(router: Router, private http: HttpClient) {
    super(router);
  }

  public getTasks() {
    return this.http.get<BaseResponse<TaskModel[]>>("/api/tasks").pipe(catchError(e => this.handleError<TaskModel[]>(e)))
  }

  public getTask(id: number) {
    return this.http.get<BaseResponse<TaskModel>>(`/api/tasks/${id}`).pipe(catchError(e => this.handleError<TaskModel>(e)))
  }

  public createTask(task: TaskModel) {
    return this.http.post<BaseResponse<TaskModel>>("/api/tasks", new RequestTaskModel(task)).pipe(catchError(e => this.handleError<TaskModel>(e)))
  }

  public updateTask(task: TaskModel) {
    return this.http.put<BaseResponse<TaskModel>>(`/api/tasks/${task.id}`, new RequestTaskModel(task)).pipe(catchError(e => this.handleError<TaskModel>(e)))
  }
}