import { Injectable } from '@angular/core';
import { BaseApiService } from './base.api.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BaseResponse } from '../models/base.response';
import { catchError } from 'rxjs';
import { DeviceTypeModel } from '../models/system.device.type.model';

@Injectable({
  providedIn: 'root'
})
export class SystemApiService extends BaseApiService {

  private _deviceActivationTimeOut: number = -1;
  public get deviceActivationTimeOut(): number{
    return this._deviceActivationTimeOut;
  }

  constructor(router: Router, private http: HttpClient) {
    super(router);
    this.getDeviceActivationTimeOut().subscribe((response) => {
      if(response.success && response.data != null){
        this._deviceActivationTimeOut = response.data;
      }
    });
  }

  public getDeviceActivationTimeOut() {
    return this.http.get<BaseResponse<number>>("/api/system/config/device-activation-timeout").pipe(catchError(e => this.handleError<number>(e)));
  }

  public getDeviceTypes() {
    return this.http.get<BaseResponse<DeviceTypeModel[]>>("/api/system/config/device-types").pipe(catchError(e => this.handleError<DeviceTypeModel[]>(e)));
  }

  public getPropertyTypes() {
    return this.http.get<BaseResponse<PropertyType[]>>("/api/system/config/property-types").pipe(catchError(e => this.handleError<PropertyType[]>(e)));
  }
}

export type PropertyType = {
  name: string;
  value: number;
}
