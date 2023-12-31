import { BaseValueModel } from "./base.value.model";
import { DeviceModel } from "./device.model";
import { DevicePropertyModel } from "./property.model";

export class TaskModel extends BaseValueModel {
    public id: number = 0;
    public name: string = "";
    public device: DeviceModel = new DeviceModel();
    public property: DevicePropertyModel = new DevicePropertyModel();
    public eventName: string = "";
    public value: string = "";
    public type: number = 0;
    public description: string = "";
    public interval: number = 0;
    public enabled: boolean = false;
}

export class RequestTaskModel extends BaseValueModel {
    public id: number = 0;
    public name: string = "";
    public deviceId: string = "";
    public devicePropertyName: string = "";
    public event: string = "";
    public value: string = "";
    public description: string = "";
    public interval: number = 0;
    public enabled: boolean = false;

    constructor(task: TaskModel) {
        super();
        this.id = task.id;
        this.name = task.name;
        this.deviceId = task.device.id;
        this.devicePropertyName = task.property.propertyName;
        this.event = task.eventName;
        this.value = task.value;
        this.description = task.description;
        this.interval = task.interval;
        this.enabled = task.enabled;
    }
}