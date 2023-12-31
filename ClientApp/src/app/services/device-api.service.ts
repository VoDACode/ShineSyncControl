import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { HttpClient } from '@angular/common/http';
import { BaseResponse } from '../models/base.response';
import { DeviceModel } from '../models/device.model';
import { catchError } from 'rxjs';
import { Router } from '@angular/router';
import { DevicePropertyModel } from '../models/property.model';
import { ShortActionModel } from '../models/short.action.model';
import { DeviceTypeModel, DeviceTypeRequestModel } from '../models/system.device.type.model';
import { TaskModel } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceApiService extends BaseApiService {
  constructor(private http: HttpClient, router: Router) { 
    super(router);
  }

  public getDevices() {
    return this.http.get<BaseResponse<DeviceModel[]>>('/api/device').pipe(catchError((e) => this.handleError<DeviceModel[]>(e)));
  }

  public getDevice(id: string) {
    return this.http.get<BaseResponse<DeviceModel>>(`/api/device/${id}`).pipe(catchError((e) => this.handleError<DeviceModel>(e)));
  }

  public updateDevice(device: DeviceModel) {
    return this.http.put<BaseResponse<DeviceModel>>(`/api/device/${device.id}`, device).pipe(catchError((e) => this.handleError<DeviceModel>(e)));
  }

  public getDeviceProperties(id: string, filter: DevicePropertyFilter | undefined = undefined) {
    let url = `/api/device/${id}/property`;
    if (filter != undefined) {
      let filterTypeParameter = filter.filterType == null ? "" : filter.filterType;
      let canBeEditedParameter = filter.canBeEdited == null ? "" : filter.canBeEdited;
      url += `?FilterType=${filterTypeParameter}&CanBeEdited=${canBeEditedParameter}`;
    }
    return this.http.get<BaseResponse<DevicePropertyModel[]>>(url).pipe(catchError((e) => this.handleError<DevicePropertyModel[]>(e)));
  }

  public getDeviceProperty(id: string, propertyId: string) {
    return this.http.get<BaseResponse<DevicePropertyModel>>(`/api/device/${id}/property/${propertyId}`).pipe(catchError((e) => this.handleError<DevicePropertyModel>(e)));
  }

  public getDeviceActions(id: string) {
    return this.http.get<BaseResponse<ShortActionModel[]>>(`/api/device/${id}/actions`).pipe(catchError((e) => this.handleError<ShortActionModel[]>(e)));
  }

  public setProperty(id: string, propertyName: string, value: any) {
    return this.http.put<BaseResponse<DevicePropertyModel>>(`/api/device/${id}/property/${propertyName}`, { value: String(value) }).pipe(catchError((e) => this.handleError<DevicePropertyModel>(e)));
  }

  public connectDevice(code: string) {
    return this.http.put<BaseResponse<any>>(`/api/device/activate?DeviceId=${code}`, {}).pipe(catchError((e) => this.handleError<any>(e)));
  }

  public registerDevice(device: DeviceTypeRequestModel) {
    return this.http.post<BaseResponse<DeviceRegisteredResponse>>('/api/device/register', device).pipe(catchError((e) => this.handleError<DeviceRegisteredResponse>(e)));
  }

  public getDeviceTasks(id: string) {
    return this.http.get<BaseResponse<TaskModel[]>>(`/api/device/${id}/tasks`).pipe(catchError((e) => this.handleError<TaskModel[]>(e)));
  }
}

type DevicePropertyFilter = {
  filterType: number | null,
  canBeEdited: boolean | null
};

export type DeviceRegisteredResponse = {
  id: string;
  token: string;
  type: string;
  registeredAt: Date;
}
