import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseValueModel } from 'src/app/models/base.value.model';
import { DeviceModel } from 'src/app/models/device.model';
import { DevicePropertyModel } from 'src/app/models/property.model';
import { TaskModel } from 'src/app/models/task.model';
import { ActionApiService } from 'src/app/services/action-api.service';
import { DeviceApiService } from 'src/app/services/device-api.service';
import { TaskApiService } from 'src/app/services/task-api.service';

@Component({
  selector: 'app-task-page',
  templateUrl: './task-page.component.html',
  styleUrls: ['./task-page.component.css']
})
export class TaskPageComponent {
  public returnTo: string = "";
  public id: string = "";
  public editMode = false;

  public task: TaskModel = new TaskModel();
  public originalTask: string = "";
  public devices: DeviceModel[] = [];
  public deviceProperties: DevicePropertyModel[] = [];
  public ableEvents: string[] = [];

  public get hasChanges(): boolean {
    if(this.originalTask == "") {
      return false;
    }
    return this.originalTask != JSON.stringify(this.task);
  }

  public get canBeSaved(): boolean {
    return this.task.name != "" && this.task.device != null && this.task.property != null && this.task.eventName != "" && this.task.property.value != "";
  }

  public get isNew(): boolean {
    return this.id == "new";
  }

  public get isEdit(): boolean {
    return this.isNew || this.editMode;
  }

  constructor(private router: Router, private route: ActivatedRoute, private deviceApiService: DeviceApiService, private actionApiService: ActionApiService, private taskApiService: TaskApiService) {
    this.route.params.subscribe(params => {
      this.id = params['id'];
      this.returnTo = params['returnTo'];
      if (!this.isNew) {
        this.loadTask();
      }else{
        this.loadDevices();
      }
    });
    this.actionApiService.getAbleEvents().subscribe(events => {
      if (events.success && events.data != undefined) {
        this.ableEvents = events.data;
        this.task.eventName = this.ableEvents[0];
      }
    });
  }

  public onSelectDevice(device: DeviceModel) {
    this.task.device = device;
    this.task.property.deviceId = '';
    this.loadDeviceProperties(device.id);
  }

  public onSelectProperty(property: DevicePropertyModel) {
    console.log(property);
    console.log(this.task.property);
    this.task.property = property;
  }

  public onselectEvent(event: string) {
    this.task.eventName = event;
  }

  public onSave() {
    if (!this.canBeSaved) {
      return;
    }
    if (this.isNew) {
      this.createTask();
    } else {
      this.updateTask();
    }
  }

  private loadTask() {
    this.taskApiService.getTask(Number(this.id)).subscribe(task => {
      if (task.success && task.data != null) {
        this.task = task.data;
        this.loadDevices();
        this.task.property.value = this.task.value;
        this.originalTask = JSON.stringify(this.task);
      }
    });
  }

  private loadDevices() {
    this.deviceApiService.getDevices().subscribe(devices => {
      if (devices.success && devices.data != null) {
        this.devices = devices.data;
        if (this.task.device.id == '') {
          this.task.device = this.devices[0] ?? new DeviceModel();
        }
        this.loadDeviceProperties(this.task.device.id);
      }
    });
  }

  private loadDeviceProperties(deviceId: string) {
    this.deviceApiService.getDeviceProperties(deviceId, {
      filterType: null,
      canBeEdited: true
    }).subscribe(properties => {
      if (properties.success && properties.data != null) {
        this.deviceProperties = properties.data;
        if (this.task.property.deviceId == '') {
          this.task.property = this.deviceProperties[0] ?? new DevicePropertyModel();
        }
        this.task.property.value = this.task.value;
      }
      for (let property of this.deviceProperties) {
        BaseValueModel.setToDefaultValueIfEmpty(property);
      }
    });
  }

  private createTask() {
    this.task.value = this.task.property.value;
    this.taskApiService.createTask(this.task).subscribe(task => {
      if (task.success && task.data != null) {
        this.onBack();
      }
    });
  }

  private updateTask() {
    this.task.value = this.task.property.value;
    this.taskApiService.updateTask(this.task).subscribe(task => {
      if (task.success && task.data != null) {
        this.onBack();
      }
    });
  }

  public onBack() {
    if (this.returnTo == "") {
      this.router.navigate(['/home']);
    } else {
      this.router.navigate([this.returnTo]);
    }
  }
}
