import { DeviceModel } from "./device.model";
import { ExpressionModel } from "./expression.model";
import { DevicePropertyModel } from "./property.model";
import { ShortActionModel } from "./short.action.model";
import { TaskModel } from "./task.model";

export class ActionModel {
    public id: number = 0;
    public name: string = '';
    public description: string = '';
    public expression: ExpressionModel = new ExpressionModel();
    public device: DeviceModel = new DeviceModel();
    public property: DevicePropertyModel = new DevicePropertyModel();
    public whenTrueTask: TaskModel = new TaskModel();
    public whenFalseTask: TaskModel | null = null;

    public static toRequestModel(model: ActionModel): ShortActionModel {
        return {
            id: model.id,
            name: model.name,
            description: model.description,
            whenTrueTaskId: model.whenTrueTask.id,
            whenFalseTaskId: model.whenFalseTask?.id ?? null,
            expression: model.expression
        }
    }
}