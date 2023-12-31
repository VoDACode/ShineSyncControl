import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DevicePropertyModel } from 'src/app/models/property.model';

@Component({
  selector: 'app-edit-property',
  templateUrl: './edit-property.component.html',
  styleUrls: ['./edit-property.component.css']
})
export class EditPropertyComponent {
  @Input()
  public property: DevicePropertyModel = new DevicePropertyModel();
  @Input()
  public ignoreReadOnly: boolean = false;
  @Input()
  public editOriginal: boolean = true;
  @Output()
  public changed: EventEmitter<PropertyChange> = new EventEmitter<PropertyChange>();

  public get isReadOnly(): boolean {
    return this.property.isReadOnly && !this.ignoreReadOnly;
  }

  public onSetProperty(value: any = null) {
    value = value == null ? this.property.value : (value ? '1' : '0');
    if (this.editOriginal) {
      this.property.value = value;
    }
    this.changed.emit({
      property: this.property,
      value: value
    });
  }
}

export type PropertyChange = { property: DevicePropertyModel, value: any };