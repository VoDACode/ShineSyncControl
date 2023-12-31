import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BaseValueModel } from 'src/app/models/base.value.model';
import { DeviceModel } from 'src/app/models/device.model';
import { ExpressionModel } from 'src/app/models/expression.model';
import { DevicePropertyModel } from 'src/app/models/property.model';
import { DeviceApiService } from 'src/app/services/device-api.service';

@Component({
  selector: 'app-expression',
  templateUrl: './expression.component.html',
  styleUrls: ['./expression.component.css']
})
export class ExpressionComponent implements OnInit {

  @Input()
  public editMode: boolean = false;
  @Input()
  public expression: ExpressionModel = new ExpressionModel();
  @Input()
  public index: number = 0;
  @Input()
  public hasNext: boolean = false;
  @Input()
  public editOrigen: boolean = false;
  @Input()
  public canRemove: boolean = false;
  @Output()
  public delete: EventEmitter<ExpressionModel> = new EventEmitter<ExpressionModel>();
  @Output()
  public edited: EventEmitter<ExpressionModel> = new EventEmitter<ExpressionModel>();

  public devices: DeviceModel[] = [];
  public deviceProperties: DevicePropertyModel[] = [];
  public comparisonOperators: {str: string, code: number}[] = 
  [
    {str: "==", code: 0},
    {str: "!=", code: 1},
    {str: ">", code: 2},
    {str: ">=", code: 3},
    {str: "<", code: 4},
    {str: "<=", code: 5}
  ];

  public selectedDevice: DeviceModel = new DeviceModel();
  public selectedProperty: DevicePropertyModel = new DevicePropertyModel();
  public selectedOperator: {str: string, code: number} = this.comparisonOperators[0];
  public expressionOperator: number = 0;

  constructor(private deviceApiService: DeviceApiService) {
  }
  ngOnInit(): void {
    if(this.editMode){
      this.loadDevices();
    }
  }

  public onExpressionOperatorChange(logic: number) {
    this.expressionOperator = logic;
    if(this.editOrigen){
      this.expression.expressionOperator = logic;
    }
    this.edited.emit(this.expression);
  }

  public onDeviceChange(device: DeviceModel) {
    this.selectedDevice = device;
    if(this.editOrigen){
      this.expression.deviceId = device.id;
    }
    this.loadProperties(device.id);
    this.edited.emit(this.expression);
  }

  public onPropertyChange(property: DevicePropertyModel) {
    this.selectedProperty = property;
    if(this.editOrigen){
      this.expression.deviceProperty = property.propertyName;
      this.expression.type = property.type;
    }
    this.edited.emit(this.expression);
  }

  public onOperatorChange(operator: {str: string, code: number}) {
    this.selectedOperator = operator;
    if(this.editOrigen){
      this.expression.operator = operator.code;
    }
    this.edited.emit(this.expression);
  }

  public onSetProperty(value: any) {
    if(this.editOrigen){
      this.expression.value = this.selectedProperty.value;
    }
    this.edited.emit(this.expression);
  }

  private loadDevices() {
    this.deviceApiService.getDevices().subscribe(response => {
      if (response.success && response.data != null) {
        this.devices = response.data;
        this.selectedDevice = this.devices[0];
        if(this.editOrigen && this.expression.deviceId.length == 0){
          this.expression.deviceId = this.devices[0].id;
        }
        this.loadProperties(this.expression.deviceId);
      }
    });
  }

  private loadProperties(deviceId: string) {
    this.deviceApiService.getDeviceProperties(deviceId).subscribe(response => {
      if (response.success && response.data != null) {
        this.deviceProperties = response.data;
        this.selectedProperty = this.deviceProperties[0];
        if(this.editOrigen && this.expression.deviceId == deviceId) {
          this.expression.deviceProperty = this.deviceProperties[0].propertyName;
          this.expression.type = this.deviceProperties[0].type;
        }
        this.selectedProperty.value = this.expression.value;
        BaseValueModel.setToDefaultValueIfEmpty(this.expression);
      }
    });
  }

  public onDelete() {
    this.delete.emit(this.expression);
  }
}
