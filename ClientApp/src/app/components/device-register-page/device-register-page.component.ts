import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DeviceTypeModel } from 'src/app/models/system.device.type.model';
import { DeviceApiService } from 'src/app/services/device-api.service';
import { PropertyType, SystemApiService } from 'src/app/services/system-api.service';

@Component({
  selector: 'app-device-register-page',
  templateUrl: './device-register-page.component.html',
  styleUrls: ['./device-register-page.component.css']
})
export class DeviceRegisterPageComponent {

  public deviceTypes: DeviceTypeModel[] = [];
  public selectedDeviceType: DeviceTypeModel = new DeviceTypeModel();

  public propertyTypes: PropertyType[] = [];

  constructor(private router: Router, private translate: TranslateService, private systemApiService: SystemApiService, private deviceApiService: DeviceApiService) {
    this.systemApiService.getPropertyTypes().subscribe((response) => {
      if (response.success && response.data != null) {
        this.propertyTypes = response.data.filter((x) => x.value != 0);
      }
    });
    this.systemApiService.getDeviceTypes().subscribe((response) => {
      if (response.success && response.data != null) {
        this.deviceTypes = response.data;
        if (this.deviceTypes.length > 0) {
          this.selectedDeviceType = this.deviceTypes[0];
        }
      }
    });
  }

  onSelectDeviceType(deviceType: DeviceTypeModel) {
    this.selectedDeviceType = deviceType;
  }

  onRegisterDevice() {
    console.log(this.selectedDeviceType);
    this.deviceApiService.registerDevice(DeviceTypeModel.toRequestModel(this.selectedDeviceType)).subscribe((response) => {
      if (response.success && response.data != null) {
        // make a file with the response.data
        const file = new Blob([JSON.stringify(response.data)], { type: 'application/json' });
        const fileURL = URL.createObjectURL(file);
        // rename the file to the device name
        const a = document.createElement('a');
        a.href = fileURL;
        a.download = `${response.data.id}.config.json`;
        a.click();
        alert(this.translate.instant('message.device-registered-successfully'));
        document.body.removeChild(a);
      } else {
        alert(response.message);
      }
    });
  }
}
