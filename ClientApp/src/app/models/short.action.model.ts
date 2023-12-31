import { ExpressionModel } from "./expression.model";

export class ShortActionModel {
    public id: number = 0;
    public name: string = '';
    public description: string = '';
    public whenTrueTaskId: number = 0;
    public whenFalseTaskId: number | null = null;
    public expression: ExpressionModel = new ExpressionModel();
}