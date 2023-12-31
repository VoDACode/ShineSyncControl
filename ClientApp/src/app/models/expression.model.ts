import { BaseValueModel } from "./base.value.model";

export class ExpressionModel extends BaseValueModel {   
    public id: number = 0;
    public deviceId: string = "";
    public deviceProperty: string = "";
    public operator: number = 0;
    public value: string = "";
    public type: number = 0;
    public subExpression: ExpressionModel | null = null;
    public expressionOperator: number = 0;
}