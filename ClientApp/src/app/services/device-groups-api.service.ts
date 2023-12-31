import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { DeviceGroupModel } from '../models/device.group.model';
import { catchError } from 'rxjs';
import { BaseResponse } from '../models/base.response';

@Injectable({
  providedIn: 'root'
})
export class DeviceGroupsApiService extends BaseApiService {

  constructor(router: Router, private http: HttpClient) {
    super(router);
  }

  public getGroups() {
    return this.http.get<BaseResponse<DeviceGroupModel[]>>(`/api/device/groups`).pipe(catchError(e => this.handleError<DeviceGroupModel[]>(e)));
  }

  public getGroup(id: number) {
    return this.http.get<BaseResponse<DeviceGroupModel>>(`/api/device/groups/${id}`).pipe(catchError(e => this.handleError<DeviceGroupModel>(e)));
  }

  public createGroup(group: DeviceGroupModel) {
    let request = {
      name: group.name,
      description: group.description,
      devices: group.devices?.map(d => d.id)
    };
    return this.http.post<BaseResponse<DeviceGroupModel>>(`/api/device/groups`, request).pipe(catchError(e => this.handleError<DeviceGroupModel>(e)));
  }

  public updateGroup(group: DeviceGroupModel) {
    let request = {
      id: group.id,
      name: group.name,
      description: group.description,
      devices: group.devices?.map(d => d.id)
    };
    return this.http.put<BaseResponse<DeviceGroupModel>>(`/api/device/groups/${request.id}`, request).pipe(catchError(e => this.handleError<DeviceGroupModel>(e)));
  }

  public deleteGroup(id: number) {
    return this.http.delete<BaseResponse<DeviceGroupModel>>(`/api/device/groups/${id}`).pipe(catchError(e => this.handleError<DeviceGroupModel>(e)));
  }

  public addDeviceToGroup(groupId: number, deviceId: string) {
    return this.http.post<BaseResponse<DeviceGroupModel>>(`/api/device/groups/${groupId}/${deviceId}`, null).pipe(catchError(e => this.handleError<DeviceGroupModel>(e)));
  }

  public removeDeviceFromGroup(groupId: number, deviceId: string) {
    return this.http.delete<BaseResponse<DeviceGroupModel>>(`/api/device/groups/${groupId}/${deviceId}`).pipe(catchError(e => this.handleError<DeviceGroupModel>(e)));
  }
}
