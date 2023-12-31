import { DeviceModel } from "./device.model";

export class DeviceGroupModel{
    public id: number = 0;
    public name: string = '';
    public description: string | null = null;
    public devices: DeviceModel[] | null = null;
}