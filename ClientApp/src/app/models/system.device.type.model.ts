export class DeviceTypeModel {
    public type: string = '';
    public properties: DevicePropertyTypeModel[] = [];

    public static toRequestModel(model: DeviceTypeModel): DeviceTypeRequestModel {
        return new DeviceTypeRequestModel(model);
    }
}

export class DevicePropertyTypeModel {
    public isReadOnly: boolean = false;
    public propertyName: string = "";
    public type: string = "";
    public typeCode: number = 0;
    public units: string | null = null;
}

export class DeviceTypeRequestModel {
    public type: string = '';
    public properties: DevicePropertyTypeRequestModel[] = [];

    constructor(deviceType: DeviceTypeModel) {
        this.type = deviceType.type;
        this.properties = deviceType.properties.map((x) => new DevicePropertyTypeRequestModel(x));
    }
}

export class DevicePropertyTypeRequestModel {
    public isReadOnly: boolean = false;
    public propertyName: string = "";
    public type: number = 0;
    public units: string | null = null;

    constructor(devicePropertyType: DevicePropertyTypeModel) {
        this.isReadOnly = devicePropertyType.isReadOnly;
        this.propertyName = devicePropertyType.propertyName;
        this.type = devicePropertyType.typeCode;
        this.units = devicePropertyType.units;
    }
}