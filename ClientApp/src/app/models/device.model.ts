export class DeviceModel {
    public id: string = '';
    public ownerId: number | undefined;
    public name: string | undefined | null;
    public type: string = 'relay';
    public description: string | undefined | null;
    public isActive: boolean = false;
    public lastSync: Date | undefined;
    public lastOnline: Date | undefined;
    public registeredAt: Date = new Date();
    public activatedAt: Date | undefined;
}

