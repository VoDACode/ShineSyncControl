import { Component, Input } from '@angular/core';
import { DeviceModel } from '../../models/device.model';

@Component({
  selector: 'app-device-item',
  templateUrl: './device-item.component.html',
  styleUrls: ['./device-item.component.css']
})
export class DeviceItemComponent {

  private static readonly DEVICE_TYPES: any = {
    light: {
      on: '<i class="icon ion-ios-lightbulb"></i>',
      off: '<i class="icon ion-ios-lightbulb-outline"></i>'
    },
    curtains: {
      on: '<img class="icon" src="/assets/img/jalousie.png"/>',
      off: 'curtains_closed'
    },
  };

  @Input() device: DeviceModel = new DeviceModel();

  public getDeviceType(): string {
    var deviceType = DeviceItemComponent.DEVICE_TYPES[this.device.type];

    if (deviceType) {
      return this.device.isActive ? deviceType.on : deviceType.off;
    }

    return 'ion-ios-lightbulb';
  }
}
