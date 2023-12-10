import { Component } from '@angular/core';
import { DeviceModel } from 'src/app/models/device.model';
import { DeviceApiService } from 'src/app/services/device-api.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent {
  public devices: DeviceModel[] = []

  constructor(private deviceApiService: DeviceApiService) {
    this.deviceApiService.getDevices().subscribe(res => {
      if (res.success && res.data) {
        this.devices = res.data;
      } else {
        alert(res.message);
      }
    });
  }
}
