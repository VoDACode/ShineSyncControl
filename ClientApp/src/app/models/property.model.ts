export class DevicePropertyModel{
    public id: number = 0;
    public deviceId: string = '';
    public propertyName: string = '';
    public value: string = '';
    public isReadOnly: boolean = false;
    public propertyUnit: string | undefined | null;
    public propertyLastSync: Date | undefined;
    public type: number = 0;
    public typeText: string = '';
}