import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { HttpClient } from '@angular/common/http';
import { BaseResponse } from '../models/base.response';
import { DeviceModel } from '../models/device.model';
import { catchError } from 'rxjs';
import { Router } from '@angular/router';
import { DevicePropertyModel } from '../models/property.model';
import { ShortActionModel } from '../models/short.action.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceApiService extends BaseApiService {
  constructor(private http: HttpClient, router: Router) { 
    super(router);
  }

  public getDevices() {
    return this.http.get<BaseResponse<DeviceModel[]>>('/api/device').pipe(catchError(this.handleError));
  }

  public getDevice(id: string) {
    return this.http.get<BaseResponse<DeviceModel>>(`/api/device/${id}`).pipe(catchError(this.handleError));
  }

  public updateDevice(device: DeviceModel) {
    return this.http.put<BaseResponse<DeviceModel>>(`/api/device/${device.id}`, device).pipe(catchError(this.handleError));
  }

  public getDeviceProperties(id: string) {
    return this.http.get<BaseResponse<DevicePropertyModel[]>>(`/api/device/${id}/property`).pipe(catchError(this.handleError));
  }

  public getDeviceProperty(id: string, propertyId: string) {
    return this.http.get<BaseResponse<DevicePropertyModel>>(`/api/device/${id}/property/${propertyId}`).pipe(catchError(this.handleError));
  }

  public getDeviceActions(id: string) {
    return this.http.get<BaseResponse<ShortActionModel[]>>(`/api/device/${id}/actions`).pipe(catchError(this.handleError));
  }
}
