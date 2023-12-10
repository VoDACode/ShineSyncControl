import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DeviceModel } from 'src/app/models/device.model';
import { DevicePropertyModel } from 'src/app/models/property.model';
import { ShortActionModel } from 'src/app/models/short.action.model';
import { DeviceApiService } from 'src/app/services/device-api.service';

@Component({
  selector: 'app-device-page',
  templateUrl: './device-page.component.html',
  styleUrls: ['./device-page.component.css']
})
export class DevicePageComponent implements OnInit {
  public device: DeviceModel = new DeviceModel();

  public deviceProperties: DevicePropertyModel[] = [];

  public deviceActions: ShortActionModel[] = [];

  public editedDevice: DeviceModel = new DeviceModel();

  public get hasChanges (): boolean {
    if(this.editedDevice.description === undefined || this.editedDevice.description === null || this.editedDevice.description === ''){
      this.editedDevice.description = null;
    }
    if(this.editedDevice.name === undefined || this.editedDevice.name === null || this.editedDevice.name === ''){
      this.editedDevice.name = null;
    }
    return JSON.stringify(this.device) !== JSON.stringify(this.editedDevice);
  } 

  public deviceId: string = "1";

  public editMode: boolean = false;

  constructor(private route: ActivatedRoute, private deviceApiService: DeviceApiService) {
    this.route.params.subscribe(params => {
      this.deviceId = params['id'];
    });
  }

  public ngOnInit() {
    this.deviceApiService.getDevice(this.deviceId).subscribe(res => {
      if (res.success && res.data) {
        this.device = res.data;
        this.editedDevice = Object.assign({}, this.device);
      }else{
        alert(res.message);
      }
    });
    this.deviceApiService.getDeviceProperties(this.deviceId).subscribe(res => {
      if (res.success && res.data) {
        this.deviceProperties = res.data;
      }else{
        alert(res.message);
      }
    });
    this.deviceApiService.getDeviceActions(this.deviceId).subscribe(res => {
      if (res.success && res.data) {
        this.deviceActions = res.data;
      }else{
        alert(res.message);
      }
    });
  }

  public onEditClick(status: boolean) {
    this.editMode = status;
    if (status) {
      this.editedDevice = Object.assign({}, this.device);
    }
  }

  public onCancelClick() {
    this.editMode = false;
  }

  public onSaveClick() {
    this.deviceApiService.updateDevice(this.editedDevice).subscribe(res => {
      if (res.success && res.data) {
        this.device = res.data;
        this.editedDevice = Object.assign({}, this.device);
        this.editMode = false;
      }else{
        alert(res.message);
      }
    });
  }

}
