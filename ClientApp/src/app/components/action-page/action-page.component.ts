import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ActionModel } from 'src/app/models/action.model';
import { BaseValueModel } from 'src/app/models/base.value.model';
import { ExpressionModel } from 'src/app/models/expression.model';
import { TaskModel } from 'src/app/models/task.model';
import { ActionApiService } from 'src/app/services/action-api.service';
import { ExpressionApiService } from 'src/app/services/expression-api.service';
import { TaskApiService } from 'src/app/services/task-api.service';
import { base64 } from 'src/app/utils/base64';

@Component({
  selector: 'app-action-page',
  templateUrl: './action-page.component.html',
  styleUrls: ['./action-page.component.css']
})
export class ActionPageComponent {
  public id: string = "";

  public action: ActionModel = new ActionModel();
  public originalActionJson: string = "";

  public targetDeviceId: string | null = null;

  public tasks: TaskModel[] = [];

  public expressions: ExpressionModel[] = [];

  private newExpressions: ExpressionModel[] = [];
  private deletedExpressions: ExpressionModel[] = [];
  private editedExpressions: ExpressionModel[] = [];

  public hasNextExpression(index: number): boolean {
    return index < this.expressions.length - 1;
  }

  public get isNew(): boolean {
    return this.id === "new";
  }

  public get hasChanges(): boolean {
    if (this.originalActionJson.length == 0) {
      return false;
    }
    return JSON.stringify(this.action) !== this.originalActionJson;
  }

  public get canBeSaved(): boolean {
    return this.expressions.length > 0 && this.action.name.length > 0 && this.action.whenTrueTask.id > 0;
  }

  constructor(private router: Router, private route: ActivatedRoute, private actionApiService: ActionApiService, private taskApiService: TaskApiService, private expressionApiService: ExpressionApiService) {
    this.route.params.subscribe(params => {
      this.id = params['id'];
      this.targetDeviceId = params['target'];
      let paramsString = params['params'];
      if (paramsString != undefined) {
        let params: ActionPageParams = JSON.parse(base64.decode(paramsString));
        this.id = params.id;
        this.action.name = params.name;
        this.action.description = params.description;
        this.targetDeviceId = params.target;
      }
      this.loadTasks();
      if (!this.isNew) {
        this.loadAction();
      }
    });
  }

  public onAddExpression() {
    let newExpression = new ExpressionModel();
    let lastExpression = this.expressions[this.expressions.length - 1];
    if (lastExpression != undefined) {
      lastExpression.subExpression = newExpression;
    }
    this.expressions.push(newExpression);
    if (!this.isNew) {
      this.newExpressions.push(newExpression);
    }
  }

  public onDeleteExpression(index: number) {
    let expression = this.expressions[index];

    if (index == 0 && expression.subExpression != null) {
      this.action.expression = expression.subExpression;
    } else {
      let lastExpression = this.expressions[index - 1];
      if (lastExpression != undefined) {
        lastExpression.subExpression = expression.subExpression;
      }
    }

    this.expressions.splice(index, 1);
    if (!this.isNew) {
      this.deletedExpressions.push(expression);
    }
  }

  public onSelectedTaskWhenTrueClick(task: TaskModel) {
    this.action.whenTrueTask = task;
  }

  public onSelectedTaskWhenFalseClick(task: TaskModel | null) {
    if (task == null) {
      this.action.whenFalseTask = null;
      return;
    }
    this.action.whenFalseTask = task;
  }

  public expressionCanBeRemoved(index: number): boolean {
    return this.expressions.length > 1;
  }

  public onExpressionEdited(expression: ExpressionModel) {

    if (this.isNew || this.newExpressions.indexOf(expression) != -1) {
      return;
    }

    let index = this.editedExpressions.indexOf(expression);
    if (index == -1) {
      this.editedExpressions.push(expression);
    } else {
      this.editedExpressions[index] = expression;
    }
  }

  public onSave() {
    if (this.isNew) {
      this.createNewAction();
    } else {
      this.updateAction();
    }
  }

  public onBack() {
    console.log(this.targetDeviceId);
    if (this.targetDeviceId != null) {
      this.router.navigate(['/device', this.targetDeviceId]);
    } else {
      this.router.navigate(['/home']);
    }
  }

  public onNewTask() {
    let params = {
      id: this.id,
      name: this.action.name,
      description: this.action.description,
      target: this.targetDeviceId ?? ""
    };
    console.log(params);
    this.router.navigate(['/task/new', {
      returnTo: '/action/' + this.id,
      params: base64.encode(JSON.stringify(params))
    }]);
  }

  private createNewAction() {
    this.action.expression = this.expressions[0];
    console.log(this.action);
    this.actionApiService.createAction(this.action).subscribe(response => {
      if (response.success && response.data != null) {
        this.router.navigate(['/action', response.data.id]);
      }
    });
  }

  private updateAction() {
    let body = {
      action: this.action,
      createInExtension: this.newExpressions,
      editedExpressions: this.editedExpressions,
      deletedExpressions: this.deletedExpressions
    }
    console.log(body);

    for (let i = 0; i < this.expressions.length; i++) {
      let expression = this.expressions[i];
      let indexInNew = this.newExpressions.indexOf(expression);
      let indexInEdited = this.editedExpressions.indexOf(expression);
      if (indexInNew != -1) {
        this.newExpressions.splice(indexInNew, 1);
        expression.value = String(expression.value);
        this.expressionApiService.createInExpression(this.expressions[i - 1].id, expression).subscribe(response => {
          if (response.success && response.data != null) {
            console.log(response.data);
            expression.id = response.data.id;
          }
        });
      } else if (indexInEdited != -1) {
        this.editedExpressions.splice(indexInEdited, 1);
        expression.value = String(expression.value);
        this.expressionApiService.updateExpression(expression).subscribe(response => {
          if (response.success && response.data != null) {
            console.log(response.data);
          }
        });
      }
    }

    for (let i = 0; i < this.deletedExpressions.length; i++) {
      let expression = this.deletedExpressions[i];
      if (this.expressions.indexOf(expression) != -1) {
        continue;
      }
      this.expressionApiService.deleteExpression(expression.id).subscribe(response => {
        if (response.success && response.data != null) {
          this.deletedExpressions.splice(i, 1);
          console.log(response.data);
        }
      });
    }

    // update action
    this.actionApiService.updateAction(this.action).subscribe(response => {
      if (response.success && response.data != null) {
        this.originalActionJson = JSON.stringify(this.action);
      }
    });
  }

  private loadAction() {
    this.actionApiService.getAction(this.id).subscribe(response => {
      if (response.success && response.data != null) {
        this.action = response.data;

        console.log(this.action);

        this.loadExpressions();
        this.originalActionJson = JSON.stringify(this.action);
      }
    });
  }

  private loadExpressions() {
    this.actionApiService.getActionExpressions(String(this.action.id)).subscribe(response => {
      if (response.success && response.data != null) {
        this.expressions = [];
        let expression: ExpressionModel | null = response.data;
        this.action.expression = expression;
        while (expression != null) {
          this.expressions.push(expression);
          BaseValueModel.setToDefaultValueIfEmpty(expression);
          expression = expression.subExpression;
        }
        this.originalActionJson = JSON.stringify(this.action);
      }
    });
  }

  private loadTasks() {
    this.taskApiService.getTasks().subscribe(tasks => {
      if (tasks.success && tasks.data != null) {
        this.tasks = tasks.data;
        if(this.isNew){
          this.action.whenTrueTask = this.tasks[0];
        }
      }
    });
  }
}

type ActionPageParams = {
  id: string,
  name: string,
  description: string,
  target: string
}
